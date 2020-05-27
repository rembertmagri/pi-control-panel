namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using System;
    using global::GraphQL.DataLoader;
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public class CpuLoadStatusType : ObjectGraphType<CpuLoadStatus>
    {
        public CpuLoadStatusType(
            IDataLoaderContextAccessor accessor,
            ICpuService cpuService,
            ILogger logger)
        {
            this.Field<DateTimeGraphType>("dateTime");

            this.Field(x => x.Processes, false, typeof(ListGraphType<CpuProcessType>)).Resolve(context => context.Source.Processes);

            this.Field(x => x.LastMinuteAverage);

            this.Field(x => x.Last5MinutesAverage);

            this.Field(x => x.Last15MinutesAverage);

            this.Field(x => x.KernelRealTime);

            this.Field(x => x.UserRealTime);

            this.Field<FloatGraphType, double>()
                .Name("TotalRealTime")
                .ResolveAsync(context =>
                {
                    logger.Debug("TotalRealTime field");

                    var cpuRealTimeLoad = context.Source;
                    var loader = accessor.Context.GetOrAddBatchLoader<DateTime, double>(
                        "GetTotalRealTimeLoadAsync",
                        cpuService.GetTotalRealTimeLoadsAsync);

                    return loader.LoadAsync(cpuRealTimeLoad.DateTime);
                });
        }
    }
}
