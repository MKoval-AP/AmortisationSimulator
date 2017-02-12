using System.Collections.Generic;
using System.Linq;
using AmortisationSimulator.Core.Input;

namespace AmortisationSimulator.Core.Engine
{
    internal class AmortisationTables : Dictionary<Deduction, AmortisationTable>
    {
        public AmortisationTables(Deduction[] creditors) : base((IDictionary<Deduction, AmortisationTable>) creditors.ToDictionary(c => c, c => new AmortisationTable(c)))
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
}