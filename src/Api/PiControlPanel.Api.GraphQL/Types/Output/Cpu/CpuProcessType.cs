namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
 
    public class CpuProcessType : ObjectGraphType<CpuProcess>
    {
        public CpuProcessType()
        {
            Field<DateTimeGraphType>("dateTime");
            Field(x => x.ProcessId);
            Field(x => x.User);
            Field(x => x.Priority);
            Field(x => x.NiceValue);
            Field(x => x.TotalMemory);
            Field(x => x.Ram);
            Field(x => x.SharedMemory);
            Field(x => x.State);
            Field(x => x.CpuPercentage);
            Field(x => x.RamPercentage);
            Field(x => x.TotalCpuTime);
            Field(x => x.Command);
        }
    }
}
