namespace PiControlPanel.Api.GraphQL.Types.Input
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Enums;

    /// <summary>
    /// The CpuMaxFrequencyLevel GraphQL input type.
    /// </summary>
    public class CpuMaxFrequencyLevelType : EnumerationGraphType<CpuMaxFrequencyLevel>
    {
    }
}
