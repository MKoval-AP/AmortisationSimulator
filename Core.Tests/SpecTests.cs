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
            var specRun = new SpecRun();
            var sim = new Simulator();

            foreach (var referenceFile in Directory.GetFiles("Specs"))
            {
                var data = new SpecContextData(referenceFile);
                data.Actual = sim.Simulate(data.Input);
                specRun.Specs.Add(data);
            }

            specRun.ValidateAndSaveResult();
        }
    }
}