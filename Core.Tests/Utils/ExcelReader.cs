using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmortisationSimulator.Core.Tests.ExcelModels;
using AmortisationSimulator.Core.Tests.Models;
using LinqToExcel;
using LinqToExcel.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmortisationSimulator.Core.Tests.Utils
{
    public class ExcelReader
    {
        const string AmortPrefix = "Amortisation_";
        private readonly ExcelQueryFactory _excel;
        private readonly List<string> _sheetNames;
        private readonly List<string> _namedRanges;
        private string CaseName { get; }

        public ExcelReader(string file)
        {
            CaseName = Path.GetFileNameWithoutExtension(file);
            _excel = new ExcelQueryFactory(file) { UsePersistentConnection = true };
            _sheetNames = _excel.GetWorksheetNames().ToList();
            _namedRanges = _excel.GetNamedRanges().ToList();
        }

        public void Fill(SpecContextData target)
        {
            target.ExcelInputData = ReadInput();
            target.ExcelCreditors = ReadCreditors();
            target.ExcelAmortisationSummary = ReadAmortisationSummary();
            target.ExcelAmortisationTables = ReadAmortisationTables(target.ExcelCreditors);
        }

        private ExcelModels.Input ReadInput()
        {
            var input = FromWorksheet<ExcelModels.Input>("Input").ToArray();
            Assert.AreEqual(1, input.Length, $"{CaseName}: Unexpected number of input data rows in Excel");
            return input.First();
        }

        private Creditor[] ReadCreditors()
        {
            var creditors = FromWorksheet<Creditor>("Creditors").Where(c => !string.IsNullOrEmpty(c.CreditorName)).ToArray();
            Assert.AreEqual(creditors.Select(c => c.CreditorName).Distinct().Count(), creditors.Length, $"{CaseName}: Creditor names should be unique");
            return creditors;
        }

        private AmortisationSummaryLine[] ReadAmortisationSummary()
        {
            var data = from asl in FromNamedRange($"{AmortPrefix}Summary")
                       where asl["TotalCreditorPayments"].Cast<decimal>() > 0
                       select
                           new AmortisationSummaryLine
                           {
                               Period = asl["Period"].Cast<int>(),
                               Date = asl["Date"].Cast<DateTime?>(),
                               ContributionAmount = asl["ContributionAmount"].Cast<decimal>(),
                               NegotiationFee = asl["NegotiationFee"].Cast<decimal>(),
                               LegalFee = asl["LegalFee"].Cast<decimal>(),
                               PdaFee = asl["PdaFee"].Cast<decimal>(),
                               DcFee = asl["DcFee"].Cast<decimal>(),
                               //creditor columns
                               TotalCreditorPayments = asl["TotalCreditorPayments"].Cast<decimal>(),
                               DistributableToCreditors = asl["DistributableToCreditors"].Cast<decimal>(),
                               UnallocatedAmount = asl["UnallocatedAmount"].Cast<decimal>()
                           };

            return data.ToArray();
        }

        private Dictionary<Creditor, AmortisationLine[]> ReadAmortisationTables(Creditor[] creditors)
        {
            var result = new Dictionary<Creditor, AmortisationLine[]>(creditors.Length);
            foreach (var creditor in creditors)
            {
                result[creditor] = FromNamedRange<AmortisationLine>($"{AmortPrefix}{creditor.CreditorName}").Where(l => l.OpeningBalance > 0).ToArray();
            }
            return result;
        }

        private void AssertSheetExists(string name) => Assert.IsTrue(_sheetNames.Contains(name), $"{CaseName}: {name} sheet not found");
        private void AssertNamedRangeExists(string name) => Assert.IsTrue(_namedRanges.Contains(name), $"{CaseName}: {name} named range not found");

        private ExcelQueryable<T> FromWorksheet<T>(string sheetName)
        {
            AssertSheetExists(sheetName);
            return _excel.Worksheet<T>(sheetName);
        }

        private ExcelQueryable<T> FromNamedRange<T>(string namedRangeName)
        {
            AssertNamedRangeExists(namedRangeName);
            return _excel.NamedRange<T>(namedRangeName);
        }

        private ExcelQueryable<Row> FromNamedRange(string namedRangeName)
        {
            AssertNamedRangeExists(namedRangeName);
            return _excel.NamedRange(namedRangeName);
        }
    }
}