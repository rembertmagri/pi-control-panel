namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public class CpuTemperatureType : ObjectGraphType<CpuTemperature>
    {
        public CpuTemperatureType()
        {
            Field("value", x => x.Temperature);
            Field<DateTimeGraphType>("dateTime");
        }
    }
}
