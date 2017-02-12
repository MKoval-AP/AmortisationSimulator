using System;

namespace AmortisationSimulator.Core.Input
{
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
        //todo: in duplum rule: +original balance (could be different from COB balance) +already paid interest (for correct TotalInterestAccrued in payment simulation)

        public Deduction()
        {
            Id = Guid.NewGuid();
        }

        public override string ToString()
        {
            return $"{CreditorName}: IntRate: {InterestRatePercentage}, OrigInst: {OriginalInstallment}, OutstBal: {OutstandingBalance}";
        }
    }
}