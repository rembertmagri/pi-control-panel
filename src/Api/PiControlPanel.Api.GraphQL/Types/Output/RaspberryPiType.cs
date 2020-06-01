namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    /// <summary>
    /// The RaspberryPi GraphQL output type.
    /// </summary>
    public class RaspberryPiType : ObjectGraphType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RaspberryPiType"/> class.
        /// </summary>
        /// <param name="chipsetService">The application layer ChipsetService.</param>
        /// <param name="cpuService">The application layer CpuService.</param>
        /// <param name="randomAccessMemoryService">The application layer RandomAccessMemoryService.</param>
        /// <param name="swapMemoryService">The application layer SwapMemoryService.</param>
        /// <param name="gpuService">The application layer GpuService.</param>
        /// <param name="diskService">The application layer DiskService.</param>
        /// <param name="operatingSystemService">The application layer OsService.</param>
        /// <param name="networkService">The application layer NetworkService.</param>
        /// <param name="logger">The NLog logger instance.</param>
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

                    return await chipsetService.GetAsync();
                });

            this.Field<Cpu.CpuType>()
                .Name("Cpu")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Cpu field");

                    return await cpuService.GetAsync();
                });

            this.Field<MemoryType<RandomAccessMemory, RandomAccessMemoryStatus>>()
                .Name("Ram")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Ram field");

                    return await randomAccessMemoryService.GetAsync();
                });

            this.Field<MemoryType<SwapMemory, SwapMemoryStatus>>()
                .Name("swapMemory")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Swap Memory field");

                    return await swapMemoryService.GetAsync();
                });

            this.Field<GpuType>()
                .Name("Gpu")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Gpu field");

                    return await gpuService.GetAsync();
                });

            this.Field<Disk.DiskType>()
                .Name("Disk")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Disk field");

                    return await diskService.GetAsync();
                });

            this.Field<Os.OsType>()
                .Name("Os")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Os field");

                    return await operatingSystemService.GetAsync();
                });

            this.Field<Network.NetworkType>()
                .Name("Network")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Network field");

                    return await networkService.GetAsync();
                });
        }
    }
}
