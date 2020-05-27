namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    public class RaspberryPiType : ObjectGraphType
    {
        public RaspberryPiType(
            IChipsetService chipsetService,
            ICpuService cpuService,
            IMemoryService<RandomAccessMemory, RandomAccessMemoryStatus> randomAccessMemoryService,
            IMemoryService<SwapMemory, SwapMemoryStatus> swapMemoryService,
            IGpuService gpuService,
            IDiskService diskService,
            IOsService operatingSystemService,
            INetworkService networkService,
            ILogger logger)
        {
            this.Field<ChipsetType>()
                .Name("Chipset")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Chipset field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await chipsetService.GetAsync();
                });

            this.Field<Cpu.CpuType>()
                .Name("Cpu")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Cpu field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await cpuService.GetAsync();
                });

            this.Field<MemoryType<RandomAccessMemory, RandomAccessMemoryStatus>>()
                .Name("Ram")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Ram field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await randomAccessMemoryService.GetAsync();
                });

            this.Field<MemoryType<SwapMemory, SwapMemoryStatus>>()
                .Name("swapMemory")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Swap Memory field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await swapMemoryService.GetAsync();
                });

            this.Field<GpuType>()
                .Name("Gpu")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Gpu field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await gpuService.GetAsync();
                });

            this.Field<Disk.DiskType>()
                .Name("Disk")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Disk field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await diskService.GetAsync();
                });

            this.Field<Os.OsType>()
                .Name("Os")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Os field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await operatingSystemService.GetAsync();
                });

            this.Field<Network.NetworkType>()
                .Name("Network")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Network field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await networkService.GetAsync();
                });
        }
    }
}
