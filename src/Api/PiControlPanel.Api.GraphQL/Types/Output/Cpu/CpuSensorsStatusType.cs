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
            this.Field(x => x.Voltage);
            this.Field(x => x.UnderVoltageDetected);
            this.Field(x => x.ArmFrequencyCapped);
            this.Field(x => x.CurrentlyThrottled);
            this.Field(x => x.SoftTemperatureLimitActive);
            this.Field(x => x.UnderVoltageOccurred);
            this.Field(x => x.ArmFrequencyCappingOccurred);
            this.Field(x => x.ThrottlingOccurred);
            this.Field(x => x.SoftTemperatureLimitOccurred);
            this.Field<DateTimeGraphType>("dateTime");
        }
    }
}
