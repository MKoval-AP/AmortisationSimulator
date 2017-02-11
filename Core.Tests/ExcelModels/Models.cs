using System;
using AmortisationSimulator.Core.Input;
using AmortisationSimulator.Core.Output;

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

        public override string ToString()
        {
            return
                $"[Input]: Strategy: {Strategy}, CA: {ContributionAmount}, NF: {NegotiationFee}, LF: {LegalFee}, DcFee-1: {DcFeePercentage1}, DcFee-2: {DcFeePercentage2}, EscalationPercentage: {EscalationPercentage}, EscalationAmount: {EscalationAmount}, EscalationDate: {EscalationDate}, SD: {StartDate}";
        }

        public SimVariables ToSimVariables()
        {
            return new SimVariables { ContributionAmount = ContributionAmount, DcFeePercentage1 = DcFeePercentage1, DcFeePercentage2 = DcFeePercentage2 };
        }
    }

    public class Creditor
    {
        public string CreditorName { get; set; }
        public string CreditorType { get; set; }
        public decimal InterestRate { get; set; }
        public decimal OriginalInstallment { get; set; }
        public decimal OutstandingBalance { get; set; }
        public DateTime? COBDate { get; set; }
        public decimal ServiceFees { get; set; }
        public decimal LinkedInsurance { get; set; }
        public decimal CustomInstallment { get; set; }
        public decimal ProRataPercentage { get; set; }
        public decimal ProRataInstallment { get; set; }

        private Deduction _deduction;

        public override string ToString()
        {
            return
                $"[Creditor]: {CreditorName}({CreditorType}), IntRate: {InterestRate}, Installment: {OriginalInstallment}, OB: {OutstandingBalance}, COBDate: {COBDate}, SF: {ServiceFees}, LI: {LinkedInsurance}, CI: {CustomInstallment}, ProRataPercentage: {ProRataPercentage}, ProRataInstallment: {ProRataInstallment}";
        }

        public Deduction ToDeduction()
        {
            //cache deduction to keep generated creditor id
            if (_deduction == null)
            {
                _deduction = new Deduction
                {
                    CreditorName = CreditorName,
                    InterestRatePercentage = InterestRate,
                    OriginalInstallment = OriginalInstallment,
                    OutstandingBalance = OutstandingBalance
                };
            }
            return _deduction;
        }
    }

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

    public class AmortisationLine
    {
        public int Period { get; set; }
        public DateTime? Date { get; set; }
        public string CreditorName { get; set; }
        public string CreditorType { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Installment { get; set; }
        public decimal AccruedInterest { get; set; }
        public decimal OriginalInstallment { get; set; }
        public decimal OutstandingBalance { get; set; }
        public DateTime? COBDate { get; set; }
        public decimal ServiceFees { get; set; }
        public decimal LinkedInsurance { get; set; }
        public decimal CustomInstallment { get; set; }
        public decimal ProRataPercentage { get; set; }
        public decimal DistributableToCreditors { get; set; }
        public decimal ProRataPercentageForSurplus { get; set; }
        public decimal CreditorSurplus { get; set; }

        public override string ToString()
        {
            return
                $"[AL] {Period}: {CreditorName}({CreditorType}), Inst: {Installment}, AccruedInterest: {AccruedInterest}, OB: {OutstandingBalance}, SF: {ServiceFees}, LI: {LinkedInsurance}, CI: {CustomInstallment}, IR: {InterestRate},  OI: {OriginalInstallment}, COBDate: {COBDate}, ProRataPercentage: {ProRataPercentage}, DistributableToCreditors: {DistributableToCreditors}, ProRataPercentageForSurplus: {ProRataPercentageForSurplus}, CreditorSurplus: {CreditorSurplus}, Date: {Date}";
        }

        public AmortisationTableLine ToSimLine()
        {
            return new AmortisationTableLine
            {
                Period = Period,
                AccruedInterest = AccruedInterest,
                Installment = Installment,
                OutstandingBalance = OutstandingBalance
            };
        }
    }
}