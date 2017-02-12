using System;
using System.Collections.Generic;
using System.Text;
using AmortisationSimulator.Core.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmortisationSimulator.Core.Tests.Models
{
    public class SpecRun
    {
        private static readonly DebugConsole DebugConsole = new DebugConsole();

        private DateTime Started { get; }
        public List<SpecContextData> Specs { get; }

        public SpecRun()
        {
            Started = DateTime.Now;
            Specs = new List<SpecContextData>();
        }

        public void ValidateAndSaveResult()
        {
            var directory = $"SpecRun_{Started.ToString("s").Replace(":", "-")}";
            var sb = new StringBuilder();
            foreach (var spec in Specs)
            {
                DebugConsole.WriteHeader($"{spec.CaseName}");
                DebugConsole.Indent();
                spec.ActualMatchesSpec = SimResultComparer.Compare(spec.Spec, spec.Actual);
                DebugConsole.Unindent();
                DebugConsole.WriteFooter($"{(spec.ActualMatchesSpec ? "passed" : "FAILED")}");
                SaveCsvFiles(directory, spec);
            }

            //comparison summary to test result
            Console.WriteLine(sb);

            Assert.IsTrue(Specs.TrueForAll(tc => tc.ActualMatchesSpec));
        }

        private static void SaveCsvFiles(string directory, SpecContextData specContextData)
        {
            //todo: save expected and actual as plain CSV files for comparison by WinMerge, for example
            //another, more complicated idea: generate new Excel file where expected and actual are next to each other. Then WinMerge won't be needed.
        }
    }
}