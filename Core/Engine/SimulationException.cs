using System;
using AmortisationSimulator.Core.Output;

namespace AmortisationSimulator.Core.Engine
{
    public class SimulationException : Exception
    {
        public readonly SolutionType SolutionType;

        public SimulationException(SolutionType solutionType, string message = null) : base(message)
        {
            SolutionType = solutionType;
        }
    }
}