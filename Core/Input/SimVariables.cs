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
        public decimal PdaFeePercentage { get; set; }
        public decimal VatPercentage { get; set; }
        public bool IsDcVatRegistered { get; set; }
        public bool IsOldPdaFee { get; set; }

        //todo: first payment date
        //todo: DC VAT registered
        //todo: negotiation fee
        //todo: legal fees
        //todo: escalations
        //todo: payment day

        public override string ToString()
        {
            return
                $"[{Strategy}]: CA: {ContributionAmount}, DcFee-1: {DcFeePercentage1.ToString("p")}, DcFee-2: {DcFeePercentage2.ToString("p")}, PdaFee: {PdaFeePercentage.ToString("p")}{Environment.NewLine}{string.Join(Environment.NewLine, Creditors.Select(c => c.ToString()))}";
        }
    }
}