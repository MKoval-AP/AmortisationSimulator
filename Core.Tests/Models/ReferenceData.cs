using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AmortisationSimulator.Core.Input;
using AmortisationSimulator.Core.Output;
using AmortisationSimulator.Core.Tests.ExcelModels;
using LinqToExcel;
using LinqToExcel.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AmortisationLine = AmortisationSimulator.Core.Output.AmortisationLine;
using AmortisationSummaryLine = AmortisationSimulator.Core.Tests.ExcelModels.AmortisationSummaryLine;

namespace AmortisationSimulator.Core.Tests.Models
{
    public class ReferenceData
    {
        private readonly ExcelQueryFactory _excel;
        private List<string> _sheetNames;
        public string CaseName { get; set; }
        public SimVariables Input { get; private set; }
        public SimResult Expected { get; private set; }
        public SimResult Actual { get; set; }
        public bool ActualMatchesExpected { get; private set; }

        public ExcelModels.Input ExcelInputData { get; private set; }
        public Creditor[] ExcelCreditors { get; private set; }
        public AmortisationSummaryLine[] ExcelAmortisationSummary { get; private set; }
        public Dictionary<Creditor, ExcelModels.AmortisationLine[]> ExcelAmortisationTables { get; private set; }

        public ReferenceData(string pathToFile)
        {
            CaseName = Path.GetFileNameWithoutExtension(pathToFile);
            _excel = new ExcelQueryFactory(pathToFile) { UsePersistentConnection = true };
            ReadFromFile();
        }

        #region Read from Excel

        private void ReadFromFile()
        {
            _sheetNames = _excel.GetWorksheetNames().ToList();

            //input
            var input = FromWorksheet<ExcelModels.Input>("Input").ToArray();
            Assert.AreEqual(1, input.Length, $"{CaseName}: Unexpected number of input data rows in Excel");
            ExcelInputData = input.First();
            Input = ExcelInputData.ToSimVariables();

            //creditors
            ExcelCreditors = FromWorksheet<Creditor>("Creditors").Where(c => !string.IsNullOrEmpty(c.CreditorName)).ToArray();
            Assert.AreEqual(
                ExcelCreditors.Select(c => c.CreditorName).Distinct().Count(),
                ExcelCreditors.Length,
                $"{CaseName}: Creditor names should be unique");
            Input.Creditors = ExcelCreditors.Select(c => c.ToDeduction()).ToArray();

            const string amortPrefix = "Amortisation";

            //amortisation summary
            ExcelAmortisationSummary = FromWorksheet<AmortisationSummaryLine>($"{amortPrefix}-Summary").Where(sl => sl.TotalCreditorPayments > 0).ToArray();

            ExcelAmortisationTables = new Dictionary<Creditor, ExcelModels.AmortisationLine[]>(ExcelCreditors.Length);
            foreach (var creditor in ExcelCreditors)
            {
                ExcelAmortisationTables[creditor] = FromWorksheet<ExcelModels.AmortisationLine>($"{amortPrefix}-{creditor.CreditorName}").Where(l => l.Period > 0).ToArray();
            }

            Expected = new SimResult(Input.Strategy, SolutionType.SolutionFound)
            {
                AmortisationSummary = new AmortisationSummary { Lines = ExcelAmortisationSummary.Select(el => el.ToSimLine()).ToArray() },
                AmortisationTables =
                    ExcelAmortisationTables.Select(
                        de => new AmortisationTable { Creditor = de.Key.ToDeduction(), Lines = de.Value.Select(al => al.ToSimLine()).ToArray() }).ToArray()
            };
        }

        private void AssertSheetExists(string name) => Assert.IsTrue(_sheetNames.Contains(name), $"{CaseName}: {name} sheet not found");

        private ExcelQueryable<T> FromWorksheet<T>(string sheetName)
        {
            AssertSheetExists(sheetName);
            return _excel.Worksheet<T>(sheetName);
        }

        #endregion

        //todo: make extendable and less copy-paste. find proper place

        #region comparison methods

        public void CompareExpectedAndActual(StringBuilder logger)
        {
            if (Expected == null)
            {
                logger.AppendIndented("Expected is null");
                ActualMatchesExpected = false;
                return;
            }

            if (Actual == null)
            {
                logger.AppendIndented("Actual is null");
                ActualMatchesExpected = false;
                return;
            }

            ActualMatchesExpected = Expected.Result == Actual.Result;
            if (!ActualMatchesExpected)
            {
                logger.AppendIndented(Actual.Message);
                logger.AppendIndented($"Simulation result mismatch: expected: {Expected.Result}, actual: {Actual.Result}");
                return;
            }

            ActualMatchesExpected = CompareTables(Expected.AmortisationTables, Actual.AmortisationTables, logger);
            if (!ActualMatchesExpected)
            {
                return;
            }

            ActualMatchesExpected = CompareSummaryTables(Expected.AmortisationSummary, Actual.AmortisationSummary, logger);
        }

        private static bool CompareTables(AmortisationTable[] expected, AmortisationTable[] actual, StringBuilder logger)
        {
            if (expected == null)
            {
                logger.AppendIndented("Expected.AmortisationTable[] is null");
                return false;
            }

            if (actual == null)
            {
                logger.AppendIndented("Actual.AmortisationTable[] is null");
                return false;
            }

            if (expected.Length != actual.Length)
            {
                logger.AppendIndented($"AmortisationTable[].Length mismatch: expected: {expected.Length}, actual: {actual.Length}");
                return false;
            }

            foreach (var expectedTable in expected)
            {
                var actualTable = actual.FirstOrDefault(a => a.Creditor.Id.Equals(expectedTable.Creditor.Id));
                if (actualTable == null)
                {
                    logger.AppendIndented(
                        $"[{expectedTable.Creditor.CreditorName}]: Creditor {expectedTable.Creditor.Id} not found in Actual.AmortisationTable[]");
                    return false;
                }

                if (!CompareTable(expectedTable, actualTable, logger))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CompareTable(AmortisationTable expected, AmortisationTable actual, StringBuilder logger)
        {
            var creditorName = expected.Creditor.CreditorName;
            if (expected.Lines.Length != actual.Lines.Length)
            {
                logger.AppendIndented(
                    $"[{creditorName}]: AmortisationTable.Lines.Length mismatch: expected: {expected.Lines.Length}, actual: {actual.Lines.Length}");
                return false;
            }

            foreach (var expectedLine in expected.Lines)
            {
                var actualLine = actual.Lines.FirstOrDefault(l => l.Period == expectedLine.Period);
                if (actualLine == null)
                {
                    logger.AppendIndented($"[{creditorName}]: Period {expectedLine.Period} not found in Actual.AmortisationTable.Lines");
                    return false;
                }

                if (!CompareAmortisationLine(expectedLine, actualLine, logger))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CompareAmortisationLine(AmortisationLine expected, AmortisationLine actual, StringBuilder logger)
        {
            if (expected.AccruedInterest != actual.AccruedInterest)
            {
                logger.AppendIndented($"AL[{expected.Period}]: AccruedInterest mismatch: expected {expected.AccruedInterest}, actual: {actual.AccruedInterest}");
                return false;
            }

            if (expected.Installment != actual.Installment)
            {
                logger.AppendIndented($"AL[{expected.Period}]: Installment mismatch: expected {expected.Installment}, actual: {actual.Installment}");
                return false;
            }

            if (expected.OutstandingBalance != actual.OutstandingBalance)
            {
                logger.AppendIndented(
                    $"AL[{expected.Period}] OutstandingBalance mismatch: expected {expected.OutstandingBalance}, actual: {actual.OutstandingBalance}");
                return false;
            }

            return true;
        }

        private static bool CompareSummaryTables(AmortisationSummary expected, AmortisationSummary actual, StringBuilder logger)
        {
            if (expected == null)
            {
                logger.AppendIndented("Expected.AmortisationSummary is null");
                return false;
            }

            if (actual == null)
            {
                logger.AppendIndented("Actual.AmortisationSummary is null");
                return false;
            }

            if (expected.Lines.Length != actual.Lines.Length)
            {
                logger.AppendIndented($"AmortisationSummary.Lines.Length mismatch: expected: {expected.Lines.Length}, actual: {actual.Lines.Length}");
                return false;
            }

            foreach (var expectedLine in expected.Lines)
            {
                var actualLine = actual.Lines.FirstOrDefault(l => l.Period == expectedLine.Period);
                if (actualLine == null)
                {
                    logger.AppendIndented($"Period {expectedLine.Period} not found in Actual.AmortisationSummary");
                    return false;
                }

                if (!CompareAmortisationSummaryLine(expectedLine, actualLine, logger))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CompareAmortisationSummaryLine(Output.AmortisationSummaryLine expected, Output.AmortisationSummaryLine actual, StringBuilder logger)
        {
            if (expected.ContributionAmount != actual.ContributionAmount)
            {
                logger.AppendIndented(
                    $"ASL[{expected.Period}]: ContributionAmount mismatch: expected {expected.ContributionAmount}, actual: {actual.ContributionAmount}");
                return false;
            }

            if (expected.DistributableToCreditors != actual.DistributableToCreditors)
            {
                logger.AppendIndented(
                    $"ASL[{expected.Period}]: DistributableToCreditors mismatch: expected {expected.DistributableToCreditors}, actual: {actual.DistributableToCreditors}");
                return false;
            }

            if (expected.DcFee != actual.DcFee)
            {
                logger.AppendIndented($"AL[{expected.Period}]: DcFee mismatch: expected {expected.DcFee}, actual: {actual.DcFee}");
                return false;
            }

            if (expected.TotalCreditorPayments != actual.TotalCreditorPayments)
            {
                logger.AppendIndented(
                    $"ASL[{expected.Period}]: TotalCreditorPayments mismatch: expected {expected.TotalCreditorPayments}, actual: {actual.TotalCreditorPayments}");
                return false;
            }

            if (expected.UnallocatedAmount != actual.UnallocatedAmount)
            {
                logger.AppendIndented(
                    $"ASL[{expected.Period}]: UnallocatedAmount mismatch: expected {expected.UnallocatedAmount}, actual: {actual.UnallocatedAmount}");
                return false;
            }

            return true;
        }

        #endregion
    }
}