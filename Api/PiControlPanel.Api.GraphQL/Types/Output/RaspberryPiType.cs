namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    public class RaspberryPiType : ObjectGraphType
    {
        public RaspberryPiType(IChipsetService chipsetService, ICpuService cpuService,
            IMemoryService<RandomAccessMemory, RandomAccessMemoryStatus> randomAccessMemoryService,
            IMemoryService<SwapMemory, SwapMemoryStatus> swapMemoryService, IGpuService gpuService,
            IDiskService diskService, IOsService osService, ILogger logger)
        {
            Field<ChipsetType>()
                .Name("Chipset")
                .ResolveAsync(async context =>
                {
                    logger.Info("Chipset field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await chipsetService.GetAsync();
                });

            Field<Cpu.CpuType>()
                .Name("Cpu")
                .ResolveAsync(async context =>
                {
                    logger.Info("Cpu field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await cpuService.GetAsync();
                });

            Field<MemoryType<RandomAccessMemory, RandomAccessMemoryStatus>>()
                .Name("Ram")
                .ResolveAsync(async context =>
                {
                    logger.Info("Ram field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await randomAccessMemoryService.GetAsync();
                });

            Field<MemoryType<SwapMemory, SwapMemoryStatus>>()
                .Name("swapMemory")
                .ResolveAsync(async context =>
                {
                    logger.Info("Swap Memory field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await swapMemoryService.GetAsync();
                });

            Field<GpuType>()
                .Name("Gpu")
                .ResolveAsync(async context =>
                {
                    logger.Info("Gpu field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await gpuService.GetAsync();
                });

            Field<Disk.DiskType>()
                .Name("Disk")
                .ResolveAsync(async context =>
                {
                    logger.Info("Disk field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await diskService.GetAsync();
                });

            Field<Os.OsType>()
                .Name("Os")
                .ResolveAsync(async context =>
                {
                    logger.Info("Os field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await osService.GetAsync();
                });
        }
    }
}
