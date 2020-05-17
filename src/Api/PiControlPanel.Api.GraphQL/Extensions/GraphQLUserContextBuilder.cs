namespace PiControlPanel.Api.GraphQL.Extensions
{
    using global::GraphQL.Server.Transports.AspNetCore;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class GraphQLUserContextBuilder : IUserContextBuilder
    {
        public Task<object> BuildUserContext(HttpContext httpContext) =>
            Task.FromResult<object>(new GraphQLUserContext() { User = httpContext.User });
    }
}
