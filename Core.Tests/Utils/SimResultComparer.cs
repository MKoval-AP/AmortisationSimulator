using System;
using System.Linq;
using AmortisationSimulator.Core.Output;

namespace AmortisationSimulator.Core.Tests.Utils
{
    //todo: make extendable (new properties compared automatically) and less copy-paste. find proper place
    public static class SimResultComparer
    {
        public static bool Compare(SimResult spec, SimResult actual)
        {
            if (spec == null)
            {
                Console.WriteLine("Spec is null");
                return false;
            }

            if (actual == null)
            {
                Console.WriteLine("Actual is null");
                return false;
            }

            var matches = spec.Result == actual.Result;
            if (!matches)
            {
                Console.WriteLine(actual.Message);
                Console.WriteLine($"Simulation result mismatch: expected: {spec.Result}, actual: {actual.Result}");
                return false;
            }

            return CompareTables(spec.AmortisationTables, actual.AmortisationTables) &&
                CompareSummaryTables(spec.AmortisationSummary, actual.AmortisationSummary);
        }

        private static bool CompareTables(AmortisationTable[] spec, AmortisationTable[] actual)
        {
            if (spec == null)
            {
                Console.WriteLine("Spec.AmortisationTable[] is null");
                return false;
            }

            if (actual == null)
            {
                Console.WriteLine("Actual.AmortisationTable[] is null");
                return false;
            }

            if (spec.Length != actual.Length)
            {
                Console.WriteLine($"AmortisationTable[].Length mismatch: expected: {spec.Length}, actual: {actual.Length}");
                return false;
            }

            foreach (var specTable in spec)
            {
                var actualTable = actual.FirstOrDefault(a => a.Creditor.Id.Equals(specTable.Creditor.Id));
                if (actualTable == null)
                {
                    Console.WriteLine($"[{specTable.Creditor.CreditorName}]: Creditor {specTable.Creditor.Id} not found in Actual.AmortisationTable[]");
                    return false;
                }

                if (!CompareTable(specTable, actualTable))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CompareTable(AmortisationTable spec, AmortisationTable actual)
        {
            var creditorName = spec.Creditor.CreditorName;
            if (spec.Lines.Length != actual.Lines.Length)
            {
                Console.WriteLine($"[{creditorName}]: AmortisationTable.Lines.Length mismatch: expected: {spec.Lines.Length}, actual: {actual.Lines.Length}");
                return false;
            }

            foreach (var specLine in spec.Lines)
            {
                var actualLine = actual.Lines.FirstOrDefault(l => l.Period == specLine.Period);
                if (actualLine == null)
                {
                    Console.WriteLine($"[{creditorName}]: Period {specLine.Period} not found in Actual.AmortisationTable.Lines");
                    return false;
                }

                if (!CompareAmortisationLine(specLine, actualLine))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CompareAmortisationLine(AmortisationLine spec, AmortisationLine actual)
        {
            if (spec.AccruedInterest != actual.AccruedInterest)
            {
                Console.WriteLine($"AL[{spec.Period}]: AccruedInterest mismatch: expected {spec.AccruedInterest}, actual: {actual.AccruedInterest}");
                return false;
            }

            if (spec.Installment != actual.Installment)
            {
                Console.WriteLine($"AL[{spec.Period}]: Installment mismatch: expected {spec.Installment}, actual: {actual.Installment}");
                return false;
            }

            if (spec.OutstandingBalance != actual.OutstandingBalance)
            {
                Console.WriteLine($"AL[{spec.Period}] OutstandingBalance mismatch: expected {spec.OutstandingBalance}, actual: {actual.OutstandingBalance}");
                return false;
            }

            return true;
        }

        private static bool CompareSummaryTables(AmortisationSummary spec, AmortisationSummary actual)
        {
            if (spec == null)
            {
                Console.WriteLine("Spec.AmortisationSummary is null");
                return false;
            }

            if (actual == null)
            {
                Console.WriteLine("Actual.AmortisationSummary is null");
                return false;
            }

            if (spec.Lines.Length != actual.Lines.Length)
            {
                Console.WriteLine($"AmortisationSummary.Lines.Length mismatch: expected: {spec.Lines.Length}, actual: {actual.Lines.Length}");
                return false;
            }

            foreach (var specLine in spec.Lines)
            {
                var actualLine = actual.Lines.FirstOrDefault(l => l.Period == specLine.Period);
                if (actualLine == null)
                {
                    Console.WriteLine($"Period {specLine.Period} not found in Actual.AmortisationSummary");
                    return false;
                }

                if (!CompareAmortisationSummaryLine(specLine, actualLine))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CompareAmortisationSummaryLine(AmortisationSummaryLine spec, AmortisationSummaryLine actual)
        {
            if (spec.ContributionAmount != actual.ContributionAmount)
            {
                Console.WriteLine($"ASL[{spec.Period}]: ContributionAmount mismatch: expected {spec.ContributionAmount}, actual: {actual.ContributionAmount}");
                return false;
            }

            if (spec.DistributableToCreditors != actual.DistributableToCreditors)
            {
                Console.WriteLine(
                    $"ASL[{spec.Period}]: DistributableToCreditors mismatch: expected {spec.DistributableToCreditors}, actual: {actual.DistributableToCreditors}");
                return false;
            }

            if (spec.DcFee != actual.DcFee)
            {
                Console.WriteLine($"AL[{spec.Period}]: DcFee mismatch: expected {spec.DcFee}, actual: {actual.DcFee}");
                return false;
            }

            if (spec.TotalCreditorPayments != actual.TotalCreditorPayments)
            {
                Console.WriteLine(
                    $"ASL[{spec.Period}]: TotalCreditorPayments mismatch: expected {spec.TotalCreditorPayments}, actual: {actual.TotalCreditorPayments}");
                return false;
            }

            if (spec.UnallocatedAmount != actual.UnallocatedAmount)
            {
                Console.WriteLine($"ASL[{spec.Period}]: UnallocatedAmount mismatch: expected {spec.UnallocatedAmount}, actual: {actual.UnallocatedAmount}");
                return false;
            }

            return true;
        }
    }
}