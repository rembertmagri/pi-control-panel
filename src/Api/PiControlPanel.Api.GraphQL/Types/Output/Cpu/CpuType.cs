namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    /// <summary>
    /// The Cpu GraphQL output type.
    /// </summary>
    public class CpuType : ObjectGraphType<Cpu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CpuType"/> class.
        /// </summary>
        /// <param name="cpuService">The application layer CpuService.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public CpuType(ICpuService cpuService, ILogger logger)
        {
            this.Field(x => x.Cores);
            this.Field(x => x.Model);
            this.Field(x => x.ScalingGovernor);
            this.Field("maxFrequency", x => x.MaximumFrequency);

            this.Field<CpuLoadStatusType>()
                .Name("LoadStatus")
                .ResolveAsync(async context =>
                {
                    logger.Debug("LoadStatus field");

                    return await cpuService.GetLastLoadStatusAsync();
                });

            this.Connection<CpuLoadStatusType>()
                .Name("LoadStatuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("LoadStatuses connection");

                    var pagingInput = context.GetPagingInput();
                    var averageLoads = await cpuService.GetLoadStatusesAsync(pagingInput);

                    return averageLoads.ToConnection();
                });

            this.Field<CpuTemperatureType>()
                .Name("Temperature")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Temperature field");

                    return await cpuService.GetLastTemperatureAsync();
                });

            this.Connection<CpuTemperatureType>()
                .Name("Temperatures")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("Temperatures connection");

                    var pagingInput = context.GetPagingInput();
                    var temperatures = await cpuService.GetTemperaturesAsync(pagingInput);

                    return temperatures.ToConnection();
                });

            this.Field<CpuFrequencyType>()
                .Name("Frequency")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Frequency field");

                    return await cpuService.GetLastFrequencyAsync();
                });

            this.Connection<CpuFrequencyType>()
                .Name("Frequencies")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("Frequencies connection");

                    var pagingInput = context.GetPagingInput();
                    var frequencies = await cpuService.GetFrequenciesAsync(pagingInput);

                    return frequencies.ToConnection();
                });
        }
    }
}
