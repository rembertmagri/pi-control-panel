namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public class CpuTemperatureType : ObjectGraphType<CpuTemperature>
    {
        public CpuTemperatureType()
        {
            this.Field("value", x => x.Temperature);
            this.Field<DateTimeGraphType>("dateTime");
        }
    }
}
