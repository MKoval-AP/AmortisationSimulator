using System;
using System.Collections.Generic;
using System.Linq;
using AmortisationSimulator.Core.Input;

namespace AmortisationSimulator.Core.Engine
{
    internal class AmortisationSummaryLine
    {
        public int Period { get; }
        public decimal ContributionAmount { get; }
        public decimal TotalCreditorPayments { get; set; }
        public decimal DcFee { get; set; }
        public decimal DistributableToCreditors => ContributionAmount - DcFee;
        public decimal UnallocatedAmount { get; set; }

        public AmortisationSummaryLine(int period, decimal contributionAmount)
        {
            Period = period;
            ContributionAmount = contributionAmount;
        }

        public override string ToString()
        {
            return
                $"[ASL] {Period}: CA: {ContributionAmount}, TotalCreditorPayments: {TotalCreditorPayments}, DcFee: {DcFee}, DistributableToCreditors: {DistributableToCreditors}, UnallocatedAmount: {UnallocatedAmount}";
        }

        public Output.AmortisationSummaryLine ToOutput()
        {
            return new Output.AmortisationSummaryLine
            {
                Period = Period,
                ContributionAmount = ContributionAmount,
                DcFee = DcFee,
                DistributableToCreditors = DistributableToCreditors,
                TotalCreditorPayments = TotalCreditorPayments,
                UnallocatedAmount = UnallocatedAmount
            };
        }
    }

    internal class AmortisationSummary : Dictionary<int, AmortisationSummaryLine>
    {
        public Output.AmortisationSummary ToOutput()
        {
            return new Output.AmortisationSummary { Lines = Values.Select(l => l.ToOutput()).OrderBy(l => l.Period).ToArray() };
        }
    }

    internal class AmortisationTable
    {
        public Deduction Creditor { get; }
        private readonly SortedList<int, AmortisationTableLine> Lines = new SortedList<int, AmortisationTableLine>();

        public AmortisationTable(Deduction creditor)
        {
            Creditor = creditor;
        }

        public bool IsPaidOut => Lines.Count > 0 && Lines.Last().Value.ClosingBalance == 0;

        public Output.AmortisationTable ToOutput()
        {
            return new Output.AmortisationTable { Creditor = Creditor, Lines = Lines.Select(l => l.Value.ToOutput()).ToArray() };
        }

        public AmortisationTableLine GetPeriod(int period)
        {
            return Lines.ContainsKey(period) ? Lines[period] : null;
        }

        public decimal AllocateToPeriod(int period, decimal installment)
        {
            if (!Lines.ContainsKey(period))
            {
                Lines.Add(period, new AmortisationTableLine(period, PreviousBalance(period), CalculateInterest(period)));
            }

            var currentPeriod = Lines[period];
            return currentPeriod.AllocateInstallment(installment);
        }

        private decimal PreviousBalance(int period)
        {
            return Lines.ContainsKey(period - 1) ? Lines[period - 1].OutstandingBalance : Creditor.OutstandingBalance;
        }

        private decimal CalculateInterest(int period)
        {
            //1/12th of yearly interest
            return Math.Round(PreviousBalance(period) * Creditor.InterestRatePercentage / 12, 2, MidpointRounding.ToEven);
        }

        public override string ToString()
        {
            return $"C: {Creditor}, Lines.Count: {Lines.Count}";
        }
    }

    internal class AmortisationTables : Dictionary<Deduction, AmortisationTable>
    {
        public AmortisationTables(Deduction[] creditors) : base(creditors.ToDictionary(c => c, c => new AmortisationTable(c)))
        {
        }

        public bool AllCreditorsPaidOut => Values.All(at => at.IsPaidOut);

        public Deduction[] CreditorsWithBalance => Values.Where(at => !at.IsPaidOut).Select(at => at.Creditor).ToArray();

        public Output.AmortisationTable[] ToOutput()
        {
            return Values.Select(at => at.ToOutput()).ToArray();
        }

        public decimal TotalCreditorPayments(int period)
        {
            return Values.Select(at => at.GetPeriod(period)).Sum(at => (at?.AllocatedInstallment).GetValueOrDefault());
        }
    }

    internal class AmortisationTableLine
    {
        public int Period { get; }
        public decimal Installment => AllocatedInstallment;
        public decimal AccruedInterest { get; private set; }
        public decimal OutstandingBalance => ClosingBalance;

        public decimal AllocatedInstallment { get; private set; }
        public decimal OpeningBalance { get; }
        public decimal InterestAccrued { get; set; }
        public decimal ClosingBalance => OpeningBalance + AccruedInterest - AllocatedInstallment;

        public AmortisationTableLine(int period, decimal openingBalance, decimal interestAccrued)
        {
            Period = period;
            OpeningBalance = openingBalance;
            InterestAccrued = interestAccrued;
            AllocatedInstallment = 0;
        }

        public decimal AllocateInstallment(decimal installment)
        {
            //take entire installment if possible
            if (ClosingBalance >= installment)
            {
                AllocatedInstallment += installment;
                return 0;
            }

            //honor outstanding balance
            var allocated = ClosingBalance;
            AllocatedInstallment += allocated;
            return installment - allocated;
        }

        public Output.AmortisationTableLine ToOutput()
        {
            return new Output.AmortisationTableLine
            {
                Period = Period,
                Installment = Installment,
                AccruedInterest = AccruedInterest,
                OutstandingBalance = OutstandingBalance
            };
        }

        public override string ToString()
        {
            return $"[AL] {Period}: AllocInst: {AllocatedInstallment}, AccruedInterest: {AccruedInterest}, Balance: {OpeningBalance} -> {ClosingBalance}";
        }
    }
}