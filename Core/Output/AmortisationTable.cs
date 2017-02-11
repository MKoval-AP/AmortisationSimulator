using System;
using System.Linq;
using System.Text;
using AmortisationSimulator.Core.Input;

namespace AmortisationSimulator.Core.Output
{
    public class SimResult
    {
        public Strategy Strategy { get; }
        public SimResultType Result { get; }
        public AmortisationSummary AmortisationSummary { get; set; }
        public AmortisationTable[] AmortisationTables { get; set; }
        public string Message { get; set; }

        public SimResult(Strategy strategy, SimResultType result)
        {
            Strategy = strategy;
            Result = result;
        }

        public override string ToString()
        {
            return
                $"[{Strategy}]: {Result} ({Message}){Environment.NewLine} {AmortisationSummary}{string.Join(Environment.NewLine, AmortisationTables.Select(at => at.ToString()))}";
        }
    }

    public enum SimResultType
    {
        SolutionFound,
        SolutionNotFound,
        SimulationException
        //todo: reasons for no solution
        ,
        StuckRemainderAllocation
    }

    public class AmortisationSummary
    {
        public AmortisationSummaryLine[] Lines { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var l in Lines)
            {
                sb.AppendLine(l.ToString());
            }
            return sb.ToString();
        }
    }

    public class AmortisationSummaryLine
    {
        public int Period { get; set; }
        public decimal ContributionAmount { get; set; }
        public decimal TotalCreditorPayments { get; set; }
        public decimal DcFee { get; set; }
        public decimal DistributableToCreditors { get; set; }
        public decimal UnallocatedAmount { get; set; }

        public override string ToString()
        {
            return
                $"[ASL] {Period}: CA: {ContributionAmount}, TotalCreditorPayments: {TotalCreditorPayments}, DcFee: {DcFee}, DistributableToCreditors: {DistributableToCreditors}, UnallocatedAmount: {UnallocatedAmount}";
        }
    }

    public class AmortisationTable
    {
        public Deduction Creditor { get; set; }
        public AmortisationTableLine[] Lines { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder($"C: {Creditor}").AppendLine();
            foreach (var l in Lines)
            {
                sb.AppendLine(l.ToString());
            }
            return sb.ToString();
        }
    }

    public class AmortisationTableLine
    {
        public int Period { get; set; }
        public decimal Installment { get; set; }
        public decimal AccruedInterest { get; set; }
        public decimal OutstandingBalance { get; set; }

        public override string ToString()
        {
            return $"[AL] {Period}: Inst: {Installment}, AccruedInterest: {AccruedInterest}, OB: {OutstandingBalance}";
        }
    }
}