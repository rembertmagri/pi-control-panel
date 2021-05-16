namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::GraphQL.Server.Transports.AspNetCore;
    using Microsoft.AspNetCore.Http;

    /// <inheritdoc/>
    public class GraphQLUserContextBuilder : IUserContextBuilder
    {
        /// <inheritdoc/>
        public Task<IDictionary<string, object>> BuildUserContext(HttpContext httpContext) =>
            Task.FromResult<IDictionary<string, object>>(new GraphQLUserContext(httpContext.User));
    }
}
