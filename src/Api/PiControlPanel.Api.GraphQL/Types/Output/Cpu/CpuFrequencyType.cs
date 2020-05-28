namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    /// <summary>
    /// The CpuFrequency GraphQL output type.
    /// </summary>
    public class CpuFrequencyType : ObjectGraphType<CpuFrequency>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CpuFrequencyType"/> class.
        /// </summary>
        public CpuFrequencyType()
        {
            this.Field("value", x => x.Frequency);
            this.Field<DateTimeGraphType>("dateTime");
        }
    }
}
