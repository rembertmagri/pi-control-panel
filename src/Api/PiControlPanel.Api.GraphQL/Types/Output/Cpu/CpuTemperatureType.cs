namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    /// <summary>
    /// The CpuTemperature GraphQL output type.
    /// </summary>
    public class CpuTemperatureType : ObjectGraphType<CpuTemperature>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CpuTemperatureType"/> class.
        /// </summary>
        public CpuTemperatureType()
        {
            this.Field("value", x => x.Temperature);
            this.Field<DateTimeGraphType>("dateTime");
        }
    }
}
