namespace PiControlPanel.Api.GraphQL.Extensions
{
    using global::GraphQL.Server.Transports.Subscriptions.Abstractions;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json.Linq;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class JwtTokenPayloadListener : IOperationMessageListener
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public JwtTokenPayloadListener(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task BeforeHandleAsync(MessageHandlingContext context)
        {
            if (MessageType.GQL_CONNECTION_INIT.Equals(context.Message?.Type))
            {
                var payload = context.Message?.Payload;
                if (payload != null)
                {
                    var authorizationTokenObject = ((JObject)payload)["Authorization"];
                    
                    if (authorizationTokenObject != null)
                    {
                        var token = authorizationTokenObject.ToString().Replace("Bearer ", string.Empty);
                        var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                        this.httpContextAccessor.HttpContext.User =
                            new ClaimsPrincipal(new ClaimsIdentity(jwtSecurityToken.Claims));
                    }
                }
            }

            var user = this.httpContextAccessor.HttpContext.User;
            context.Properties["GraphQLUserContext"] = new GraphQLUserContext() { User = user };

            return Task.CompletedTask;
        }

        public Task HandleAsync(MessageHandlingContext context)
        {
            return Task.CompletedTask;
        }

        public Task AfterHandleAsync(MessageHandlingContext context)
        {
            return Task.CompletedTask;
        }
    }
}
