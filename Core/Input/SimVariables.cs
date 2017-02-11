using System;

namespace AmortisationSimulator.Core.Input
{
    public class SimVariables
    {
        public decimal ContributionAmount { get; set; }
        public decimal DcFeePercentage1 { get; set; }
        public decimal DcFeePercentage2 { get; set; }
        //todo: first payment date
        //todo: PDA fee
        //todo: DC VAT registered
        //todo: negotiation fee
        //todo: legal fees
        //todo: escalations
        //todo: payment day
    }

    public class Deduction
    {
        public readonly Guid Id;
        public string CreditorName { get; set; }
        public decimal InterestRatePercentage { get; set; }
        public decimal OriginalInstallment { get; set; }
        public decimal OutstandingBalance { get; set; }
        //todo: creditor type
        //todo: service fees
        //todo: linked insurance
        //todo: custom installment
        //todo: COB date

        public Deduction()
        {
            Id = Guid.NewGuid();
        }
    }
}