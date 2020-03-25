namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.DataLoader;
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using System;

    public class CpuRealTimeLoadType : ObjectGraphType<CpuRealTimeLoad>
    {
        public CpuRealTimeLoadType(IDataLoaderContextAccessor accessor,
            ICpuService cpuService,
            ILogger logger)
        {
            Field<DateTimeGraphType>("dateTime");
            Field(x => x.Kernel);
            Field(x => x.User);
            Field<FloatGraphType, double>()
                .Name("Total")
                .ResolveAsync(context =>
                {
                    logger.Info("Total field");

                    var cpuRealTimeLoad = context.Source;
                    var loader = accessor.Context.GetOrAddBatchLoader<DateTime, double>(
                        "GetTotalRealTimeLoadAsync",
                        cpuService.GetTotalRealTimeLoadsAsync);

                    return loader.LoadAsync(cpuRealTimeLoad.DateTime);
                });
        }
    }
}
