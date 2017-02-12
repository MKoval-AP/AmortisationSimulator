using System;

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
        public decimal DcFee { get; set; }
        public decimal DistributableToCreditors { get; set; }
        public decimal CreditorSurplus { get; set; }

        public override string ToString()
        {
            return
                $"[ASL] {Period}: CA: {ContributionAmount}, TotalCreditorPayments: {TotalCreditorPayments}, NF: {NegotiationFee}, LF: {LegalFee}, DcFee: {DcFee}, DistributableToCreditors: {DistributableToCreditors}, CreditorSurplus: {CreditorSurplus},  Date: {Date}";
        }

        public Output.AmortisationSummaryLine ToSimLine()
        {
            return new Output.AmortisationSummaryLine
            {
                Period = Period,
                ContributionAmount = ContributionAmount,
                DcFee = DcFee,
                DistributableToCreditors = DistributableToCreditors,
                TotalCreditorPayments = TotalCreditorPayments,
                UnallocatedAmount = CreditorSurplus
            };
        }
    }
}