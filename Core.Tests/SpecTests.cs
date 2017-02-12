using System.IO;
using AmortisationSimulator.Core.Engine;
using AmortisationSimulator.Core.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmortisationSimulator.Core.Tests
{
    [TestClass]
    public class SpecTests
    {
        [TestMethod]
        public void RunAllSpecs()
        {
            var testRun = new TestRun();
            var sim = new Simulator();
            //for each xlsx file in Reference
            foreach (var referenceFile in Directory.GetFiles("Specs"))
            {
                //read file
                var data = new ReferenceData(referenceFile);
                testRun.TestCases.Add(data);
                data.Actual = sim.Simulate(data.Input);
            }

            TestUtils.ValidateTestRun(testRun);
        }
    }
}