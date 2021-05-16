namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.Security.Claims;
    using global::GraphQL;
    using global::GraphQL.Authorization;
    using global::GraphQL.Instrumentation;
    using global::GraphQL.Server;
    using global::GraphQL.Server.Transports.Subscriptions.Abstractions;
    using global::GraphQL.Validation;
    using LightInject;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NLog;
    using PiControlPanel.Api.GraphQL.Middlewares;
    using PiControlPanel.Api.GraphQL.Schemas;
    using PiControlPanel.Api.GraphQL.Types.Output;
    using PiControlPanel.Application.SecureShell;
    using PiControlPanel.Application.Services;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Contracts.Constants;
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
        /// <param name="hostingEnvironment">The hosting environment reference.</param>
        /// <returns>The altered service collection.</returns>
        public static IServiceCollection AddCustomGraphQL(
            this IServiceCollection services,
            IWebHostEnvironment hostingEnvironment)
        {
            return services
                .AddGraphQL(
                    options =>
                    {
                        options.EnableMetrics = hostingEnvironment.IsDevelopment();
                    })
                .AddSystemTextJson()

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
                .AddTransient(typeof(IGraphQLExecuter<>), typeof(InstrumentingGraphQLExecutor<>));
        }

        /// <summary>
        /// Adds required services to GraphQL.
        /// </summary>
        /// <param name="container">The original service container.</param>
        /// <returns>The altered service container.</returns>
        public static IServiceContainer AddGraphQLServicesDependency(this IServiceContainer container)
        {
            container.RegisterSingleton<IHttpContextAccessor, HttpContextAccessor>();
            container.RegisterSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            container.RegisterTransient<IValidationRule, AuthorizationValidationRule>();

            container.RegisterSingleton(s =>
            {
                var authSettings = new AuthorizationSettings();

                authSettings.AddPolicy(
                    AuthorizationPolicyName.AuthenticatedPolicy,
                    p => p
                        .RequireClaim(CustomClaimTypes.IsAnonymous, new string[] { bool.FalseString })
                        .RequireClaim(CustomClaimTypes.IsAuthenticated, new string[] { bool.TrueString })
                        .RequireClaim(ClaimTypes.Role, new string[] { Roles.User }));
                authSettings.AddPolicy(
                    AuthorizationPolicyName.SuperUserPolicy,
                    p => p
                        .RequireClaim(CustomClaimTypes.IsAnonymous, new string[] { bool.FalseString })
                        .RequireClaim(CustomClaimTypes.IsAuthenticated, new string[] { bool.TrueString })
                        .RequireClaim(ClaimTypes.Role, new string[] { Roles.SuperUser }));

                return authSettings;
            });

            container.RegisterSingleton<IDocumentExecuter, DocumentExecuter>();
            container.RegisterTransient<IOperationMessageListener, JwtPayloadListener>();
            container.Register<IFieldMiddleware, LoggerMiddleware>();
            container.Register<RaspberryPiType>();
            container.Register<ControlPanelQuery>();
            container.Register<ControlPanelMutation>();
            container.Register<ControlPanelSubscription>();
            container.Register<ControlPanelSchema>();

            return container;
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
