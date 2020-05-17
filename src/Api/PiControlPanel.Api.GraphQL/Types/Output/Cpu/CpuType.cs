namespace PiControlPanel.Api.GraphQL.Types.Output.Cpu
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public class CpuType : ObjectGraphType<Cpu>
    {
        public CpuType(ICpuService cpuService, ILogger logger)
        {
            Field(x => x.Cores);
            Field(x => x.Model);
            Field("maxFrequency", x => x.MaximumFrequency);

            Field<CpuLoadStatusType>()
                .Name("LoadStatus")
                .ResolveAsync(async context =>
                {
                    logger.Debug("LoadStatus field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await cpuService.GetLastLoadStatusAsync();
                });

            Connection<CpuLoadStatusType>()
                .Name("LoadStatuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("LoadStatuses connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var averageLoads = await cpuService.GetLoadStatusesAsync(pagingInput);

                    return averageLoads.ToConnection();
                });

            Field<CpuTemperatureType>()
                .Name("Temperature")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Temperature field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await cpuService.GetLastTemperatureAsync();
                });

            Connection<CpuTemperatureType>()
                .Name("Temperatures")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("Temperatures connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var temperatures = await cpuService.GetTemperaturesAsync(pagingInput);

                    return temperatures.ToConnection();
                });

            Field<CpuFrequencyType>()
                .Name("Frequency")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Frequency field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await cpuService.GetLastFrequencyAsync();
                });

            Connection<CpuFrequencyType>()
                .Name("Frequencies")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("Frequencies connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var frequencies = await cpuService.GetFrequenciesAsync(pagingInput);

                    return frequencies.ToConnection();
                });
        }
    }
}
