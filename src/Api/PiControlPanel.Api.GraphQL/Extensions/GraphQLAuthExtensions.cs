namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System;
    using global::GraphQL.Authorization;
    using global::GraphQL.Validation;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Contains extension methods for GraphQL authorization.
    /// </summary>
    public static class GraphQLAuthExtensions
    {
        /// <summary>
        /// Adds custom GraphQL authorization services to the service collection.
        /// </summary>
        /// <param name="services">The original service collection.</param>
        /// <param name="configure">The authorization settings and policies.</param>
        public static void AddGraphQLAuth(this IServiceCollection services, Action<AuthorizationSettings> configure)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            services.AddTransient<IValidationRule, AuthorizationValidationRule>();

            services.TryAddSingleton(s =>
            {
                var authSettings = new AuthorizationSettings();
                configure(authSettings);
                return authSettings;
            });
        }
    }
}
