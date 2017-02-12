using System.Collections.Generic;
using System.Linq;

namespace AmortisationSimulator.Core.Engine
{
    internal class AmortisationSummary : Dictionary<int, AmortisationSummaryLine>
    {
        public Output.AmortisationSummary ToOutput()
        {
            return new Output.AmortisationSummary { Lines = Values.Select(l => l.ToOutput()).OrderBy(l => l.Period).ToArray() };
        }
    }
}