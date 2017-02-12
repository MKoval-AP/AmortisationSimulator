namespace AmortisationSimulator.Core.Output
{
    public class AmortisationSummaryLine
    {
        public int Period { get; set; }
        public decimal ContributionAmount { get; set; }
        public decimal TotalCreditorPayments { get; set; }
        public decimal DcFee { get; set; }
        public decimal DistributableToCreditors { get; set; }
        public decimal UnallocatedAmount { get; set; }
        public decimal Total => TotalCreditorPayments + DcFee + UnallocatedAmount;

        public override string ToString()
        {
            return
                $"[{Period}]\tCA: {ContributionAmount}\tTCP: {TotalCreditorPayments}\tDcFee: {DcFee}\tDTC: {DistributableToCreditors}\tTotal: {Total}\tUA: {UnallocatedAmount}";
        }
    }
}