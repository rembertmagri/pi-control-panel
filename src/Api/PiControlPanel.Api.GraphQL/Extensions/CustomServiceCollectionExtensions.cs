namespace PiControlPanel.Api.GraphQL.Extensions
{
    using global::GraphQL;
    using global::GraphQL.Http;
    using global::GraphQL.Server;
    using global::GraphQL.Server.Internal;
    using global::GraphQL.Server.Transports.Subscriptions.Abstractions;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NLog;
    using PiControlPanel.Api.GraphQL.Schemas;
    using PiControlPanel.Api.GraphQL.Types.Output;
    using PiControlPanel.Application.SecureShell;
    using PiControlPanel.Application.Services;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    /// <summary>
    /// Contains extension methods for GraphQL services.
    /// </summary>
    public static class CustomServiceCollectionExtensions
    {
        /// <summary>
        /// Adds custom GraphQL configuration to the service collection.
        /// </summary>
        /// <param name="services">The original service collection.</param>
        /// <param name="webHostEnvironment">The instance of the web host environment.</param>
        /// <returns>The altered service collection.</returns>
        public static IServiceCollection AddCustomGraphQL(
            this IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            return services
                .AddGraphQL(
                    options =>
                    {
                        // Show stack traces in exceptions. Don't turn this on in production.
                        options.ExposeExceptions = webHostEnvironment.IsDevelopment();

                        options.EnableMetrics = true;
                    })

                // Adds all graph types in the current assembly with a scoped lifetime.
                .AddGraphTypes(ServiceLifetime.Scoped)

                // Adds ConnectionType<T>, EdgeType<T> and PageInfoType.
                .AddRelayGraphTypes()

                // Add a user context from the HttpContext and make it available in field resolvers.
                // Custom object required for Claims.
                .AddUserContextBuilder<GraphQLUserContextBuilder>()

                // Add required services for GraphQL web sockets (support for subscriptions).
                .AddWebSockets()

                // Add GraphQL data loader to reduce the number of calls to our repository.
                .AddDataLoader()
                .Services
                .AddSingleton<IDocumentExecuter, DocumentExecuter>()
                .AddSingleton<IDocumentWriter, DocumentWriter>()
                .AddTransient<IOperationMessageListener, JwtPayloadListener>()
                .AddTransient(typeof(IGraphQLExecuter<>), typeof(InstrumentingGraphQLExecutor<>))
                .AddScoped<RaspberryPiType>()
                .AddScoped<ControlPanelQuery>()
                .AddScoped<ControlPanelMutation>()
                .AddScoped<ControlPanelSubscription>()
                .AddScoped<ControlPanelSchema>();
        }

        /// <summary>
        /// Registers all required services to the collection.
        /// </summary>
        /// <param name="services">The original service collection.</param>
        /// <param name="configuration">The instance of the application configuration.</param>
        /// <param name="logger">The instance of the application logger.</param>
        /// <returns>The altered service collection.</returns>
        public static IServiceCollection AddRequiredServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger)
        {
            return services
                .AddScoped<ISshService, SshService>()
                .AddScoped<ISecurityService, SecurityService>()
                .AddScoped<IControlPanelService, ControlPanelService>()
                .AddScoped<IChipsetService, ChipsetService>()
                .AddScoped<ICpuService, CpuService>()
                .AddScoped<IMemoryService<RandomAccessMemory, RandomAccessMemoryStatus>, MemoryService<RandomAccessMemory, RandomAccessMemoryStatus>>()
                .AddScoped<IMemoryService<SwapMemory, SwapMemoryStatus>, MemoryService<SwapMemory, SwapMemoryStatus>>()
                .AddScoped<IGpuService, GpuService>()
                .AddScoped<IDiskService, DiskService>()
                .AddScoped<IOsService, OsService>()
                .AddScoped<INetworkService, NetworkService>()
                .AddSingleton<IConfiguration>(factory => configuration)
                .AddSingleton<ILogger>(factory => logger);
        }
    }
}
