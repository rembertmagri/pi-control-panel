namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    /// <summary>
    /// The CpuSensorsStatus GraphQL output type.
    /// </summary>
    public class CpuSensorsStatusType : ObjectGraphType<CpuSensorsStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CpuSensorsStatusType"/> class.
        /// </summary>
        public CpuSensorsStatusType()
        {
            this.Field(x => x.Temperature);
            this.Field<DateTimeGraphType>("dateTime");
        }
    }
}
