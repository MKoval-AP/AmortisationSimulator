using System;

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

        /// <summary>
        ///     includes old pda fee
        /// </summary>
        public decimal AllocatedInstallment { get; private set; }

        //AllocatedInstallment includes old pda fee
        public decimal ClosingBalance => OpeningBalance + AccruedInterest - (AllocatedInstallment - OldPdaFee);

        public decimal OldPdaFee { get; set; }

        public AmortisationLine(int period, decimal openingBalance, decimal accruedInterest, string creditorName)
        {
            Period = period;
            OpeningBalance = openingBalance;
            AccruedInterest = accruedInterest;
            CreditorName = creditorName;
            AllocatedInstallment = 0;
        }

        public decimal AllocateInstallment(decimal installment, bool isOldPdaFee)
        {
            if (ClosingBalance == 0)
            {
                return installment;
            }

            //take entire installment if needed
            if (ClosingBalance >= installment)
            {
                AllocatedInstallment += installment;
                OldPdaFee = isOldPdaFee ? CalculateOldPdaFee(AllocatedInstallment, true) : 0;
                return 0;
            }

            //honor outstanding balance (but also take old pda fee)
            var oldPdaFee = isOldPdaFee ? CalculateOldPdaFee(ClosingBalance, true) : 0;
            var allocated = ClosingBalance + oldPdaFee;
            if (allocated > installment)
            {
                throw new NotImplementedException("Edge case not implemented: we have money to pay (balance), but not (balance and old pda fee)");
            }

            AllocatedInstallment += allocated;
            OldPdaFee = oldPdaFee;
            return installment - allocated;
        }

        private decimal CalculateOldPdaFee(decimal paymentAmount, bool includesFee)
        {
            if (paymentAmount == 0)
            {
                return 0;
            }

            const decimal fee_1 = 7.98m;
            const decimal fee_2 = 17.10m;
            const decimal fee_3 = 28.50m;
            var threshold_1 = 200m;
            var threshold_2 = 500m;

            if (!includesFee)
            {
                threshold_1 -= fee_1;
                threshold_2 -= fee_2;
            }

            if (paymentAmount < threshold_1)
            {
                return fee_1;
            }

            if (paymentAmount >= threshold_1 && paymentAmount < threshold_2)
            {
                return fee_2;
            }

            return fee_3;
        }

        public Output.AmortisationLine ToOutput()
        {
            return new Output.AmortisationLine
            {
                Period = Period,
                Installment = AllocatedInstallment,
                AccruedInterest = AccruedInterest,
                ClosingBalance = ClosingBalance,
                OldPdaFee = OldPdaFee
            };
        }

        public override string ToString()
        {
            return
                $"[{CreditorName}-{Period}]: AllocInst: {AllocatedInstallment}|{OldPdaFee}, AccrInt: {AccruedInterest}, Balance: {OpeningBalance} -> {ClosingBalance}";
        }
    }
}