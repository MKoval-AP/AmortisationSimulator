using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AmortisationSimulator.Core.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmortisationSimulator.Core.Tests.Models
{
    public class SpecRun
    {
        private static readonly DebugConsole Console = new DebugConsole();

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
                Console.WriteHeader($"{spec.CaseName}");
                Console.Indent();
                spec.ActualMatchesSpec = SimResultComparer.Compare(spec.Spec, spec.Actual);
                Console.Unindent();
                Console.WriteFooter($"{(spec.ActualMatchesSpec ? "passed" : "FAILED")}");

                if (spec.ActualMatchesSpec)
                {
                    Debug.WriteLine(spec.Actual);
                }
                else
                {
                    Debug.WriteLine("SPEC:");
                    Debug.WriteLine(spec.Spec);
                    Debug.WriteLine("ACTUAL:");
                    Debug.WriteLine(spec.Actual);
                }

                SaveCsvFiles(directory, spec);
            }

            //comparison summary to test result
            System.Console.WriteLine(sb);

            Assert.IsTrue(Specs.TrueForAll(tc => tc.ActualMatchesSpec));
        }

        private static void SaveCsvFiles(string directory, SpecContextData specContextData)
        {
            //todo: save expected and actual as plain CSV files for comparison by WinMerge, for example
            //another, more complicated idea: generate new Excel file where expected and actual are next to each other. Then WinMerge won't be needed.
        }
    }
}