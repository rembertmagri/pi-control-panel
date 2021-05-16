namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.Linq;
    using System.Security.Claims;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Models;

    /// <summary>
    /// Contains extension methods to retrieve the business user context from GraphQL user context.
    /// </summary>
    public static class GraphQLUserContextExtensions
    {
        /// <summary>
        /// Creates the UserContext from the GraphQLUserContext.
        /// </summary>
        /// <param name="claimsPrincipal">The user claims.</param>
        /// <returns>The business user context created from the GraphQL user context.</returns>
        public static UserContext GetUserContext(this ClaimsPrincipal claimsPrincipal)
        {
            var userContext = new UserContext();

            var isAnonymousClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.IsAnonymous);
            if (isAnonymousClaim != null && bool.TryParse(isAnonymousClaim.Value, out bool isAnonymous))
            {
                userContext.IsAnonymous = isAnonymous;
            }

            var usernameClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Username);
            if (usernameClaim != null)
            {
                userContext.Username = usernameClaim.Value ?? string.Empty;
            }

            var passwordClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Password);
            if (passwordClaim != null)
            {
                userContext.Password = passwordClaim.Value ?? string.Empty;
            }

            userContext.IsSuperUser = claimsPrincipal.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == Roles.SuperUser);

            return userContext;
        }
    }
}
