using System;
using System.Linq;

namespace AmortisationSimulator.Core.Input
{
    public class SimVariables
    {
        public Strategy Strategy { get; set; }
        public decimal ContributionAmount { get; set; }
        public decimal DcFeePercentage1 { get; set; }
        public decimal DcFeePercentage2 { get; set; }
        public Deduction[] Creditors { get; set; }

        //todo: first payment date
        //todo: PDA fee
        //todo: DC VAT registered
        //todo: negotiation fee
        //todo: legal fees
        //todo: escalations
        //todo: payment day

        public override string ToString()
        {
            return
                $"[{Strategy}]: CA: {ContributionAmount}, DcFee-1: {DcFeePercentage1}, DcFee-2: {DcFeePercentage2}{Environment.NewLine}{string.Join($"{Environment.NewLine}", Creditors.Select(c => c.ToString()))}";
        }
    }

    public enum Strategy
    {
        ProRata
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

        public override string ToString()
        {
            return $"{CreditorName}: IntRate: {InterestRatePercentage}, Inst: {OriginalInstallment}, OB: {OutstandingBalance} {Id}";
        }
    }
}