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
        private SimVariables _variables;
        private AmortisationSummary _amortisationSummary;
        private AmortisationTables _amortisationTables;

        private int CurrentPeriod { get; set; }
        private AmortisationSummaryLine CurrentLine => _amortisationSummary[CurrentPeriod];
        private decimal CurrentContributionAmount => CurrentLine.ContributionAmount;
        private decimal CurrentDcFeePercentage => CurrentPeriod <= 24 ? _variables.DcFeePercentage1 : _variables.DcFeePercentage2;

        private void InitFields(SimVariables variables)
        {
            _variables = variables;
            _amortisationTables = new AmortisationTables(_variables.Creditors);
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
                    if (CurrentPeriod == MaxPeriods)
                    {
                        throw new SimulationException(SimResultType.SolutionNotFound);
                    }

                    AllocateCurrentPeriod();

                    CurrentPeriod++;
                }

                return CreateSolution(SimResultType.SolutionFound);
            }
            catch (SimulationException simEx)
            {
                return CreateSolution(simEx.SolutionType);
            }
            catch (Exception ex)
            {
                return CreateSolution(SimResultType.SimulationException, ex.Message);
            }
        }

        private void AllocateCurrentPeriod()
        {
            _amortisationSummary.Add(CurrentPeriod, new AmortisationSummaryLine(CurrentPeriod, _variables.ContributionAmount));
            CurrentLine.DcFee = CurrentContributionAmount * CurrentDcFeePercentage;
            CurrentLine.UnallocatedAmount = AllocateCreditors(CurrentLine.DistributableToCreditors);
            CurrentLine.TotalCreditorPayments = _amortisationTables.TotalCreditorPayments(CurrentPeriod);
        }

        private decimal AllocateCreditors(decimal distributableToCreditors)
        {
            var notPaidOutCreditors = _amortisationTables.CreditorsWithBalance;
            //allocate pro rata
            //honor outstanding balance
            var remainder = AllocateProRata(distributableToCreditors, notPaidOutCreditors);

            //allocate surplus if any & still possible
            var epoch = 0;
            while (remainder > 0 && !_amortisationTables.AllCreditorsPaidOut)
            {
                if (epoch == 5)
                {
                    throw new SimulationException(SimResultType.StuckRemainderAllocation);
                }

                var stillNotPaidOut = _amortisationTables.CreditorsWithBalance;
                remainder = AllocateProRata(remainder, stillNotPaidOut);
                epoch++;
            }

            return remainder;
        }

        private decimal AllocateProRata(decimal distributableToCreditors, Deduction[] notPaidOutCreditors)
        {
            decimal remainder = 0;
            //calculate pro rata installments
            var proRataInstallments = GetProRataInstallments(distributableToCreditors, notPaidOutCreditors);
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
                throw new NotImplementedException("Rounding error correction");
            }

            return result;
        }

        private SimResult CreateSolution(SimResultType solutionType, string message = null)
        {
            return new SimResult(_variables.Strategy, solutionType)
            {
                Message = message,
                AmortisationSummary = _amortisationSummary.ToOutput(),
                AmortisationTables = _amortisationTables.ToOutput()
            };
        }
    }

    public class SimulationException : Exception
    {
        public readonly SimResultType SolutionType;

        public SimulationException(SimResultType solutionType)
        {
            SolutionType = solutionType;
        }
    }
}