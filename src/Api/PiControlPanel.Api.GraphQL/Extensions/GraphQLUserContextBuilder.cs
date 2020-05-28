namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.Threading.Tasks;
    using global::GraphQL.Server.Transports.AspNetCore;
    using Microsoft.AspNetCore.Http;

    /// <inheritdoc/>
    public class GraphQLUserContextBuilder : IUserContextBuilder
    {
        /// <inheritdoc/>
        public Task<object> BuildUserContext(HttpContext httpContext) =>
            Task.FromResult<object>(new GraphQLUserContext() { User = httpContext.User });
    }
}
