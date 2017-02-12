using System;
using AmortisationSimulator.Core.Output;

namespace AmortisationSimulator.Core.Tests.ExcelModels
{
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

        public Output.AmortisationLine ToSimLine()
        {
            return new Output.AmortisationLine
            {
                Period = Period,
                AccruedInterest = AccruedInterest,
                Installment = Installment,
                OutstandingBalance = OutstandingBalance
            };
        }
    }
}