using System;
using System.Collections.Generic;
using System.Linq;
using AmortisationSimulator.Core.Input;
using AmortisationSimulator.Core.Output;

namespace AmortisationSimulator.Core.Engine
{
    public class Simulator
    {
        public const int MaxPeriods = 360;
        private SimVariables Variables { get; set; }
        private AmortisationSummary _amortisationSummary;
        private AmortisationTables _amortisationTables;

        private int CurrentPeriod { get; set; }
        private AmortisationSummaryLine CurrentLine => _amortisationSummary[CurrentPeriod];
        private decimal CurrentContributionAmount => CurrentLine.ContributionAmount;
        private decimal CurrentDcFeePercentage => CurrentPeriod <= 24 ? Variables.DcFeePercentage1 : Variables.DcFeePercentage2;

        private void InitFields(SimVariables variables)
        {
            Variables = variables;
            _amortisationTables = new AmortisationTables(Variables.Creditors);
            _amortisationSummary = new AmortisationSummary();
        }

        public SimResult Simulate(SimVariables variables)
        {
            try
            {
                InitFields(variables);
                CurrentPeriod = 1;
                while (!_amortisationTables.AllCreditorsPaidOut)
                {
                    if (CurrentPeriod > MaxPeriods)
                    {
                        throw new SimulationException(SolutionType.ExceededMaxPeriods);
                    }

                    AllocateCurrentPeriod();

                    CurrentPeriod++;
                }

                return CreateSolution(SolutionType.SolutionFound);
            }
            catch (SimulationException simEx)
            {
                return CreateSolution(simEx.SolutionType, simEx.Message);
            }
            catch (Exception ex)
            {
                return CreateSolution(SolutionType.SimulationException, ex.Message);
            }
        }

        private void AllocateCurrentPeriod()
        {
            _amortisationSummary.Add(CurrentPeriod, new AmortisationSummaryLine(CurrentPeriod, Variables.ContributionAmount));
            //todo: based on dates. especially if the first DC Fee payment was made before the first period date (migrated consumer, for example)
            CurrentLine.DcFee = CurrentContributionAmount * CurrentDcFeePercentage;
            CurrentLine.PdaFee = CurrentContributionAmount * Variables.PdaFeePercentage;
            //todo: careful when implementing escalation. escalation should go to surplus, not to DistributableToCreditors
            AllocateCreditors(CurrentLine.DistributableToCreditors);
            CurrentLine.TotalCreditorPayments = _amortisationTables.TotalCreditorPayments(CurrentPeriod);
        }

        private decimal AllocateCreditors(decimal distributableToCreditors)
        {
            var notPaidOutCreditors = _amortisationTables.CreditorsWithBalance;
            var remainder = AllocateCustomInstallments(distributableToCreditors, notPaidOutCreditors);

            //allocate pro rata
            //honor outstanding balance
            remainder = AllocateProRata(remainder, notPaidOutCreditors);

            //allocate surplus if any & still possible
            var epoch = 0;
            while (remainder > 0 && !_amortisationTables.AllCreditorsPaidOut)
            {
                if (epoch == 5)
                {
                    throw new SimulationException(SolutionType.StuckInRemainderAllocation);
                }

                switch (Variables.Strategy)
                {
                    case Strategy.ProRata:
                        remainder = AllocateSurplusProRata(remainder);
                        break;
                    case Strategy.Snowball:
                        remainder = AllocateSurplusSnowball(remainder);
                        break;
                    default:
                        throw new NotImplementedException($"Strategy {Variables.Strategy} is not implemented");
                }
                epoch++;
            }
            return remainder;
        }

        private decimal AllocateCustomInstallments(decimal distributableToCreditors, Deduction[] notPaidOutCreditors)
        {
            if (!notPaidOutCreditors.Any(c => c.CustomInstallment > 0))
            {
                return distributableToCreditors;
            }

            //todo: skip custom installments if any NF/LF not paid yet
            var ciSum = notPaidOutCreditors.Sum(c => c.CustomInstallment);
            if (ciSum > distributableToCreditors)
            {
                throw new SimulationException(SolutionType.TotalCustomInstallmentsMoreThanDistributableToCreditors);
            }

            var totalAllocated = 0m;
            foreach (var ciCreditor in notPaidOutCreditors.Where(c => c.CustomInstallment > 0))
            {
                totalAllocated += ciCreditor.CustomInstallment;
                totalAllocated -= _amortisationTables[ciCreditor].AllocateToPeriod(CurrentPeriod, ciCreditor.CustomInstallment);
            }

            return distributableToCreditors - totalAllocated;
        }

        private decimal AllocateSurplusSnowball(decimal remainder)
        {
            var creditorsWithBalance = _amortisationTables.CreditorsWithBalance;
            if (!creditorsWithBalance.Any())
            {
                return remainder;
            }
            var creditorBalances = creditorsWithBalance.Select(c => new { Creditor = c, Balance = _amortisationTables[c].CurrentBalance }).ToArray();
            var minBalance = creditorBalances.Min(cb => cb.Balance);
            var creditor = creditorBalances.First(cb => cb.Balance == minBalance).Creditor;

            return _amortisationTables[creditor].LastPeriod().AllocateInstallment(remainder);
            //todo: keep allocating
        }

        private decimal AllocateSurplusProRata(decimal remainder)
        {
            return AllocateProRata(remainder, _amortisationTables.CreditorsWithBalance, true);
        }

        private decimal AllocateProRata(decimal amount, Deduction[] notPaidOutCreditors, bool isSurplus = false)
        {
            if (!isSurplus)
            {
                //exclude custom installment creditors from allocation (they've got custom installment already, so giving them pro rata is not fair)
                notPaidOutCreditors = notPaidOutCreditors.Where(c => c.CustomInstallment == 0).ToArray();
                if (!notPaidOutCreditors.Any())
                {
                    return amount;
                }
            }

            decimal remainder = 0;
            //calculate pro rata installments
            var proRataInstallments = GetProRataInstallments(amount, notPaidOutCreditors);
            foreach (var creditor in notPaidOutCreditors)
            {
                remainder += _amortisationTables[creditor].AllocateToPeriod(CurrentPeriod, proRataInstallments[creditor]);
            }
            return remainder;
        }

        private Dictionary<Deduction, decimal> GetProRataInstallments(decimal availableAmount, Deduction[] notPaidOutCreditors)
        {
            var totalInstallments = notPaidOutCreditors.Sum(c => c.OriginalInstallment);
            var result = notPaidOutCreditors.ToDictionary(
                c => c,
                c => Math.Round(availableAmount * c.OriginalInstallment / totalInstallments, 2, MidpointRounding.ToEven));
            //round and adjust if necessary
            var total = result.Values.Sum();
            if (total != availableAmount)
            {
                throw new NotImplementedException("Rounding error correction not implemented");
            }

            return result;
        }

        private SimResult CreateSolution(SolutionType solutionType, string message = null)
        {
            var result = new SimResult(Variables.Strategy, solutionType)
            {
                Message = message,
                AmortisationSummary = _amortisationSummary.ToOutput(),
                AmortisationTables = _amortisationTables.ToOutput()
            };

            //Debug.WriteLine(result);

            if (solutionType == SolutionType.SolutionFound)
            {
                ValidateSolution(result);
            }

            return result;
        }

        private void ValidateSolution(SimResult result)
        {
            foreach (var summaryLine in _amortisationSummary.Values)
            {
                summaryLine.Validate();
            }
        }
    }
}