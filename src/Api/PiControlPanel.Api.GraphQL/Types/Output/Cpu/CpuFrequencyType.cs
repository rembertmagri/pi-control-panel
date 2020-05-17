namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public class CpuFrequencyType : ObjectGraphType<CpuFrequency>
    {
        public CpuFrequencyType()
        {
            Field("value", x => x.Frequency);
            Field<DateTimeGraphType>("dateTime");
        }
    }
}
