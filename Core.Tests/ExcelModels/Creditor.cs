using System;
using AmortisationSimulator.Core.Input;

namespace AmortisationSimulator.Core.Tests.ExcelModels
{
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
                    OutstandingBalance = OutstandingBalance,
                    CustomInstallment = CustomInstallment
                };
            }
            return _deduction;
        }
    }
}