using System;
using System.Collections.Generic;
using System.Linq;
using AmortisationSimulator.Core.Input;

namespace AmortisationSimulator.Core.Engine
{
    internal class AmortisationTable
    {
        public Deduction Creditor { get; }
        private readonly SortedList<int, AmortisationLine> Lines = new SortedList<int, AmortisationLine>();

        public AmortisationTable(Deduction creditor)
        {
            Creditor = creditor;
        }

        public bool IsPaidOut => Creditor.OutstandingBalance == 0 || (Lines.Count > 0 && Lines.Last().Value.ClosingBalance == 0);

        public AmortisationLine LastPeriod() => (Lines.Count > 0 ? Lines.Last().Value : null);

        public AmortisationLine GetPeriod(int period)
        {
            return Lines.ContainsKey(period) ? Lines[period] : null;
        }

        public decimal AllocateToPeriod(int period, decimal installment)
        {
            if (!Lines.ContainsKey(period))
            {
                var newPeriod = new AmortisationLine(period, PreviousBalance(period), CalculateInterest(period), Creditor.CreditorName);
                Lines.Add(newPeriod.Period, newPeriod);
            }

            var currentPeriod = Lines[period];
            return currentPeriod.AllocateInstallment(installment);
        }

        public decimal CurrentBalance => (Lines.Count > 0 ? Lines.Last().Value.ClosingBalance : Creditor.OutstandingBalance);

        private decimal PreviousBalance(int period)
        {
            return Lines.ContainsKey(period - 1) ? Lines[period - 1].ClosingBalance : Creditor.OutstandingBalance;
        }

        private decimal CalculateInterest(int period)
        {
            //todo: in duplum
            //todo: fix to "COB Date" - "period date" interest calculation
            if (period == 1)
            {
                return 0;
            }

            //1/12th of yearly interest
            var interest = PreviousBalance(period) * Creditor.InterestRatePercentage / 12.0m;
            return Math.Round(interest, 2, MidpointRounding.ToEven);
        }

        public Output.AmortisationTable ToOutput()
        {
            return new Output.AmortisationTable { Creditor = Creditor, Lines = Lines.Select(l => l.Value.ToOutput()).ToArray() };
        }

        public override string ToString()
        {
            return $"C: {Creditor}, Lines.Count: {Lines.Count}";
        }
    }
}