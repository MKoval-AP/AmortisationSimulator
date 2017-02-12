using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmortisationSimulator.Core.Input;
using AmortisationSimulator.Core.Output;
using AmortisationSimulator.Core.Tests.ExcelModels;
using AmortisationSimulator.Core.Tests.Utils;
using AmortisationLine = AmortisationSimulator.Core.Tests.ExcelModels.AmortisationLine;
using AmortisationSummaryLine = AmortisationSimulator.Core.Tests.ExcelModels.AmortisationSummaryLine;

namespace AmortisationSimulator.Core.Tests.Models
{
    public class SpecContextData
    {
        public string CaseName { get; set; }
        public SimVariables Input { get; }
        public SimResult Spec { get; private set; }
        public SimResult Actual { get; set; }
        public bool ActualMatchesSpec { get; set; }

        public ExcelModels.Input ExcelInputData { get; set; }
        public Creditor[] ExcelCreditors { get; set; }
        public AmortisationSummaryLine[] ExcelAmortisationSummary { get; set; }
        public Dictionary<Creditor, AmortisationLine[]> ExcelAmortisationTables { get; set; }

        public SpecContextData(string pathToFile)
        {
            CaseName = Path.GetFileNameWithoutExtension(pathToFile);

            new ExcelReader(pathToFile).Fill(this);

            Input = ExcelInputData.ToSimVariables();
            Input.Creditors = ExcelCreditors.Select(c => c.ToDeduction()).ToArray();

            Spec = new SimResult(Input.Strategy, SolutionType.SolutionFound)
            {
                AmortisationSummary = new AmortisationSummary { Lines = ExcelAmortisationSummary.Select(el => el.ToSimLine()).ToArray() },
                AmortisationTables =
                    ExcelAmortisationTables.Select(
                        de => new AmortisationTable { Creditor = de.Key.ToDeduction(), Lines = de.Value.Select(al => al.ToSimLine()).ToArray() }).ToArray()
            };
        }
    }
}