namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.DataLoader;
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using System;

    public class CpuLoadStatusType : ObjectGraphType<CpuLoadStatus>
    {
        public CpuLoadStatusType(
            IDataLoaderContextAccessor accessor,
            ICpuService cpuService,
            ILogger logger)
        {
            Field<DateTimeGraphType>("dateTime");

            Field(x => x.Processes, false, typeof(ListGraphType<CpuProcessType>)).Resolve(context => context.Source.Processes);

            Field(x => x.LastMinuteAverage);

            Field(x => x.Last5MinutesAverage);

            Field(x => x.Last15MinutesAverage);

            Field(x => x.KernelRealTime);

            Field(x => x.UserRealTime);

            Field<FloatGraphType, double>()
                .Name("TotalRealTime")
                .ResolveAsync(context =>
                {
                    logger.Trace("TotalRealTime field");

                    var cpuRealTimeLoad = context.Source;
                    var loader = accessor.Context.GetOrAddBatchLoader<DateTime, double>(
                        "GetTotalRealTimeLoadAsync",
                        cpuService.GetTotalRealTimeLoadsAsync);

                    return loader.LoadAsync(cpuRealTimeLoad.DateTime);
                });
        }
    }
}
