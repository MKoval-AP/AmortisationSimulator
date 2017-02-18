namespace AmortisationSimulator.Core.Output
{
    public enum SolutionType
    {
        SolutionFound,
        ExceededMaxPeriods,
        SimulationException,
        StuckInRemainderAllocation,
        TotalCustomInstallmentsMoreThanDistributableToCreditors
    }
}