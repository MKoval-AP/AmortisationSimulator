namespace AmortisationSimulator.Core.Engine
{
    internal class AmortisationLine
    {
        public string CreditorName { get; }
        public int Period { get; }
        public decimal OpeningBalance { get; }

        /// <summary>
        ///     Interest accrued from the previous period date (or COB date, if this is first period) to this period date
        /// </summary>
        public decimal AccruedInterest { get; }

        public decimal AllocatedInstallment { get; private set; }

        public decimal ClosingBalance => OpeningBalance + AccruedInterest - AllocatedInstallment;

        public AmortisationLine(int period, decimal openingBalance, decimal accruedInterest, string creditorName)
        {
            Period = period;
            OpeningBalance = openingBalance;
            AccruedInterest = accruedInterest;
            CreditorName = creditorName;
            AllocatedInstallment = 0;
        }

        public decimal AllocateInstallment(decimal installment)
        {
            //take entire installment if possible
            if (ClosingBalance >= installment)
            {
                AllocatedInstallment += installment;
                return 0;
            }

            //honor outstanding balance
            var allocated = ClosingBalance;
            AllocatedInstallment += allocated;
            return installment - allocated;
        }

        public Output.AmortisationLine ToOutput()
        {
            return new Output.AmortisationLine
            {
                Period = Period,
                Installment = AllocatedInstallment,
                AccruedInterest = AccruedInterest,
                ClosingBalance = ClosingBalance
            };
        }

        public override string ToString()
        {
            return $"[{CreditorName}-{Period}]: AllocInst: {AllocatedInstallment}, AccrInt: {AccruedInterest}, Balance: {OpeningBalance} -> {ClosingBalance}";
        }
    }
}