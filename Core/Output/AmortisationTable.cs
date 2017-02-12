using System.Text;
using AmortisationSimulator.Core.Input;

namespace AmortisationSimulator.Core.Output
{
    public class AmortisationTable
    {
        public Deduction Creditor { get; set; }
        public AmortisationLine[] Lines { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder(Creditor.ToString()).AppendLine();
            foreach (var l in Lines)
            {
                sb.AppendLine(l.ToString());
            }
            return sb.ToString();
        }
    }
}