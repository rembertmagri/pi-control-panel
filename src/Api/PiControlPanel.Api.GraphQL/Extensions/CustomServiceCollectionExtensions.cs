﻿namespace PiControlPanel.Api.GraphQL.Extensions
{
    using global::GraphQL;
    using global::GraphQL.Http;
    using global::GraphQL.Server;
    using global::GraphQL.Server.Internal;
    using global::GraphQL.Server.Transports.Subscriptions.Abstractions;
    using LightInject;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using PiControlPanel.Api.GraphQL.Schemas;
    using PiControlPanel.Api.GraphQL.Types.Output;

    /// <summary>
    /// Extension methods for GraphQL dependency injection
    /// </summary>
    public static class CustomServiceCollectionExtensions
    {
        /// <summary>
        /// The GraphQL user context for the current request
        /// </summary>
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
                // Add required services for GraphQL web sockets (support for subscriptions)
                .AddWebSockets()
                // Add GraphQL data loader to reduce the number of calls to our repository.
                .AddDataLoader()
                .Services
                .AddTransient<IOperationMessageListener, JwtTokenPayloadListener>()
                .AddTransient(typeof(IGraphQLExecuter<>), typeof(InstrumentingGraphQLExecutor<>));
        }

        public static IServiceContainer AddGraphQLServicesDependency(this IServiceContainer container)
        {
            // Sets LightInject as GraphQL's dependency resolver
            container.RegisterSingleton<IDependencyResolver>(s => new FuncDependencyResolver(container.GetInstance));

            container.RegisterSingleton<IDocumentExecuter, DocumentExecuter>();
            container.RegisterSingleton<IDocumentWriter, DocumentWriter>();

            container.Register<RaspberryPiType>();
            container.Register<ControlPanelQuery>();
            container.Register<ControlPanelMutation>();
            container.Register<ControlPanelSubscription>();
            container.Register<ControlPanelSchema>();

            return container;
        }
    }
}
