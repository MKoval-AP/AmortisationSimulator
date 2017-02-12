using System;
using System.Collections.Generic;

namespace AmortisationSimulator.Core.Tests.Models
{
    public class TestRun
    {
        public DateTime Started { get; }
        public List<ReferenceData> TestCases { get; }
        public string DirectoryName => $"TestRun_{Started.ToString("s").Replace(":", "-")}";

        public TestRun()
        {
            Started = DateTime.Now;
            TestCases = new List<ReferenceData>();
        }
    }
}