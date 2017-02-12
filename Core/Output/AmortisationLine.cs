namespace AmortisationSimulator.Core.Output
{
    public class AmortisationLine
    {
        public int Period { get; set; }
        public decimal Installment { get; set; }
        public decimal AccruedInterest { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }

        public override string ToString()
        {
            return $"[{Period}]\tInst: {Installment}\tCB: {ClosingBalance}\tAccrInt: {AccruedInterest}";
        }
    }
}