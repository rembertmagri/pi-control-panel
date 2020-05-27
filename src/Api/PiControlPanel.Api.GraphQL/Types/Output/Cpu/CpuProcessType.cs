﻿namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public class CpuProcessType : ObjectGraphType<CpuProcess>
    {
        public CpuProcessType()
        {
            this.Field<DateTimeGraphType>("dateTime");
            this.Field(x => x.ProcessId);
            this.Field(x => x.User);
            this.Field(x => x.Priority);
            this.Field(x => x.NiceValue);
            this.Field(x => x.TotalMemory);
            this.Field(x => x.Ram);
            this.Field(x => x.SharedMemory);
            this.Field(x => x.State);
            this.Field(x => x.CpuPercentage);
            this.Field(x => x.RamPercentage);
            this.Field(x => x.TotalCpuTime);
            this.Field(x => x.Command);
        }
    }
}
