using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmortisationSimulator.Core.Input;
using AmortisationSimulator.Core.Output;
using AmortisationSimulator.Core.Tests.ExcelModels;
using LinqToExcel;
using LinqToExcel.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AmortisationLine = AmortisationSimulator.Core.Tests.ExcelModels.AmortisationLine;
using AmortisationSummaryLine = AmortisationSimulator.Core.Tests.ExcelModels.AmortisationSummaryLine;

namespace AmortisationSimulator.Core.Tests.Models
{
    public class SpecContextData
    {
        private readonly ExcelQueryFactory _excel;
        private List<string> _sheetNames;
        public string CaseName { get; set; }
        public SimVariables Input { get; private set; }
        public SimResult Spec { get; private set; }
        public SimResult Actual { get; set; }
        public bool ActualMatchesSpec { get; set; }

        public ExcelModels.Input ExcelInputData { get; private set; }
        public Creditor[] ExcelCreditors { get; private set; }
        public AmortisationSummaryLine[] ExcelAmortisationSummary { get; private set; }
        public Dictionary<Creditor, AmortisationLine[]> ExcelAmortisationTables { get; private set; }

        public SpecContextData(string pathToFile)
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

            ExcelAmortisationTables = new Dictionary<Creditor, AmortisationLine[]>(ExcelCreditors.Length);
            foreach (var creditor in ExcelCreditors)
            {
                ExcelAmortisationTables[creditor] = FromWorksheet<AmortisationLine>($"{amortPrefix}-{creditor.CreditorName}").Where(l => l.Period > 0).ToArray();
            }

            Spec = new SimResult(Input.Strategy, SolutionType.SolutionFound)
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
    }
}