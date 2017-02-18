using System;

namespace AmortisationSimulator.Core.Tests.ExcelModels
{
    public class AmortisationLine
    {
        public int Period { get; set; }
        public DateTime? Date { get; set; }
        public decimal Installment { get; set; }
        public decimal ServiceFees { get; set; }
        public decimal LinkedInsurance { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal OldPdaFee { get; set; }

        /// <summary>
        ///     includes old PDA fee
        /// </summary>
        public decimal InstallmentRaw { get; set; }

        public decimal AccruedInterest { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }

        public Output.AmortisationLine ToSimLine()
        {
            return new Output.AmortisationLine
            {
                Period = Period,
                AccruedInterest = AccruedInterest,
                Installment = InstallmentRaw == 0 ? Installment : InstallmentRaw,
                OpeningBalance = OpeningBalance,
                ClosingBalance = ClosingBalance,
                OldPdaFee = OldPdaFee
            };
        }
    }
}