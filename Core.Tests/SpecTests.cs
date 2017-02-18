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
            Run(Directory.GetFiles(@"..\..\..\Specs", "*.xlsx"));
        }

        [TestMethod]
        public void RunOne()
        {
            Run(new[] { @"..\..\..\Specs\OldPdaFee-ProRata.xlsx " });
        }

        private void Run(string[] files)
        {
            var specRun = new SpecRun();
            var sim = new Simulator();

            foreach (var spec in files)
            {
                //skip Excel temp files
                if (Path.GetFileNameWithoutExtension(spec).StartsWith("~"))
                {
                    continue;
                }

                var data = new SpecContextData(spec);
                data.Actual = sim.Simulate(data.Input);
                specRun.Specs.Add(data);
            }

            specRun.ValidateAndSaveResults();
        }
    }
}