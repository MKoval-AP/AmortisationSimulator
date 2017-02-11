using AmortisationSimulator.Core.Input;

namespace AmortisationSimulator.Core.Output
{
    public class SimResult
    {
        public SimResultType Result { get; set; }
        public AmortisationSummary AmortisationSummary { get; set; }
        public AmortisationTable[] AmortisationTables { get; set; }
    }

    public enum SimResultType
    {
        SolutionFound,
        SolutionNotFound
        //todo: reasons for no solution
    }

    public class AmortisationSummary
    {
        public AmortisationSummaryLine[] Lines { get; set; }
    }

    public class AmortisationSummaryLine
    {
        public int Period { get; set; }
        public decimal ContributionAmount { get; set; }
        public decimal TotalCreditorPayments { get; set; }
        public decimal DcFee { get; set; }
        public decimal DistributableToCreditors { get; set; }
        public decimal UnallocatedAmount { get; set; }
    }

    public class AmortisationTable
    {
        public Deduction Creditor { get; set; }
        public AmortisationTableLine[] Lines { get; set; }
    }

    public class AmortisationTableLine
    {
        public int Period { get; set; }
        public decimal Installment { get; set; }
        public decimal AccruedInterest { get; set; }
        public decimal OutstandingBalance { get; set; }
    }
}