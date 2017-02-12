using System;
using System.Collections.Generic;

namespace AmortisationSimulator.Core.Tests.ExcelModels
{
    public class AmortisationSummaryLine
    {
        public int Period { get; set; }
        public DateTime? Date { get; set; }
        public decimal ContributionAmount { get; set; }
        public decimal TotalCreditorPayments { get; set; }
        public decimal NegotiationFee { get; set; }
        public decimal LegalFee { get; set; }
        public decimal PdaFee { get; set; }
        public decimal DcFee { get; set; }
        public decimal DistributableToCreditors { get; set; }
        public decimal UnallocatedAmount { get; set; }

        public Dictionary<string, CreditorPeriodSummary> CreditorSummaries = new Dictionary<string, CreditorPeriodSummary>();

        public Output.AmortisationSummaryLine ToSimLine()
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

    public class CreditorPeriodSummary
    {
        public decimal Installment { get; set; }
        public decimal AccruedInterest { get; set; }
        public decimal ClosingBalance { get; set; }
    }
}