namespace AmortisationSimulator.Core.Engine
{
    internal class AmortisationSummaryLine
    {
        public int Period { get; }
        public decimal ContributionAmount { get; }
        public decimal TotalCreditorPayments { get; set; }
        public decimal DcFee { get; set; }
        public decimal PdaFee { get; set; }
        public decimal DistributableToCreditors => ContributionAmount - DcFee - PdaFee;
        public decimal UnallocatedAmount => ContributionAmount - TotalCreditorPayments - DcFee - PdaFee;

        public AmortisationSummaryLine(int period, decimal contributionAmount)
        {
            Period = period;
            ContributionAmount = contributionAmount;
        }

        public override string ToString()
        {
            return
                $"[ASL-{Period}] CA: {ContributionAmount}, TCP: {TotalCreditorPayments}, DcFee: {DcFee}, DTC: {DistributableToCreditors}, UA: {UnallocatedAmount}";
        }

        public Output.AmortisationSummaryLine ToOutput()
        {
            return new Output.AmortisationSummaryLine
            {
                Period = Period,
                ContributionAmount = ContributionAmount,
                DcFee = DcFee,
                PdaFee = PdaFee,
                DistributableToCreditors = DistributableToCreditors,
                TotalCreditorPayments = TotalCreditorPayments,
                UnallocatedAmount = UnallocatedAmount
            };
        }
    }
}