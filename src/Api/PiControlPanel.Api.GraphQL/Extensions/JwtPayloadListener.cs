namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using global::GraphQL.Server.Transports.Subscriptions.Abstractions;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json.Linq;

    /// <inheritdoc/>
    public class JwtPayloadListener : IOperationMessageListener
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtPayloadListener"/> class,
        /// a GraphQL custom payload listener to handle JSON Web Token.
        /// </summary>
        /// <param name="httpContextAccessor">The Http context accessor that contains the HTTP context.</param>
        public JwtPayloadListener(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
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
            context.Properties["GraphQLUserContext"] = new GraphQLUserContext(user);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task HandleAsync(MessageHandlingContext context)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task AfterHandleAsync(MessageHandlingContext context)
        {
            return Task.CompletedTask;
        }
    }
}
