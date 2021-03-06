﻿using System;
using AmortisationSimulator.Core.Input;

namespace AmortisationSimulator.Core.Tests.ExcelModels
{
    public class Input
    {
        public DateTime? StartDate { get; set; }
        public decimal ContributionAmount { get; set; }
        public decimal NegotiationFee { get; set; }
        public decimal LegalFee { get; set; }
        public decimal DcFeePercentage1 { get; set; }
        public decimal DcFeePercentage2 { get; set; }
        public decimal EscalationPercentage { get; set; }
        public decimal EscalationAmount { get; set; }
        public DateTime? EscalationDate { get; set; }
        public string Strategy { get; set; }
        public decimal PdaFeePercentage { get; set; }
        public decimal VatPercentage { get; set; }
        public bool IsDcVatRegistered { get; set; }
        public bool IsOldPdaFee { get; set; }

        public override string ToString()
        {
            return
                $"[Input]: Strategy: {Strategy}, CA: {ContributionAmount}, NF: {NegotiationFee}, LF: {LegalFee}, DcFee-1: {DcFeePercentage1.ToString("p")}, DcFee-2: {DcFeePercentage2.ToString("p")}, PdaFee: {PdaFeePercentage.ToString("p")}, EscalationPercentage: {EscalationPercentage}, EscalationAmount: {EscalationAmount}, EscalationDate: {EscalationDate}, SD: {StartDate}";
        }

        public SimVariables ToSimVariables()
        {
            return new SimVariables
            {
                Strategy = (Strategy) Enum.Parse(typeof (Strategy), Strategy),
                ContributionAmount = ContributionAmount,
                DcFeePercentage1 = DcFeePercentage1,
                DcFeePercentage2 = DcFeePercentage2,
                PdaFeePercentage = PdaFeePercentage,
                VatPercentage = VatPercentage,
                IsDcVatRegistered = IsDcVatRegistered,
                IsOldPdaFee = IsOldPdaFee
            };
        }
    }
}