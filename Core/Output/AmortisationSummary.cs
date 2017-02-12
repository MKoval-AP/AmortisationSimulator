using System.Text;

namespace AmortisationSimulator.Core.Output
{
    public class AmortisationSummary
    {
        public AmortisationSummaryLine[] Lines { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var l in Lines)
            {
                sb.AppendLine(l.ToString());
            }
            return sb.ToString();
        }
    }
}