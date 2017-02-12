using System;
using System.Linq;
using System.Text;
using AmortisationSimulator.Core.Input;

namespace AmortisationSimulator.Core.Output
{
    public class SimResult
    {
        public Strategy Strategy { get; }
        public SolutionType Result { get; }
        public AmortisationSummary AmortisationSummary { get; set; }
        public AmortisationTable[] AmortisationTables { get; set; }
        public string Message { get; set; }

        public SimResult(Strategy strategy, SolutionType result)
        {
            Strategy = strategy;
            Result = result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder($"[{Strategy}]: {Result} ({Message})").AppendLine(Environment.NewLine);
            sb.AppendLine(AmortisationSummary.ToString());
            sb.AppendLine(
                string.Join(Environment.NewLine, AmortisationTables.Select(at => at.ToString())));
            return sb.ToString();
        }
    }
}