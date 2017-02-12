using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmortisationSimulator.Core.Tests.Models
{
    public static class TestUtils
    {
        public static void ValidateTestRun(TestRun testRun)
        {
            var sb = new StringBuilder();
            foreach (var referenceData in testRun.TestCases)
            {
                sb.AppendHeader($"{referenceData.CaseName}");
                referenceData.CompareExpectedAndActual(sb);
                sb.AppendFooter($"{(referenceData.ActualMatchesExpected ? "passed" : "FAILED")}");
                //write full input/output to file(s)
                SaveToFile(testRun.DirectoryName, referenceData);
            }

            //comparison summary to test result
            Console.WriteLine(sb);

            Assert.IsTrue(testRun.TestCases.TrueForAll(tc => tc.ActualMatchesExpected));
        }

        private static void SaveToFile(string directoryName, ReferenceData referenceData)
        {
            //todo: implement
        }
    }
}