namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.Linq;
    using System.Security.Claims;
    using PiControlPanel.Domain.Contracts.Constants;
    using PiControlPanel.Domain.Models;

    public static class GraphQLUserContextExtensions
    {
        public static BusinessContext GetBusinessContext(this GraphQLUserContext graphQLUserContext)
        {
            var businessContext = new BusinessContext();

            var isAnonymousClaim = graphQLUserContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.IsAnonymous);
            if (isAnonymousClaim != null && bool.TryParse(isAnonymousClaim.Value, out bool isAnonymous))
            {
                businessContext.IsAnonymous = isAnonymous;
            }

            var usernameClaim = graphQLUserContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Username);
            if (usernameClaim != null)
            {
                businessContext.Username = usernameClaim.Value ?? string.Empty;
            }

            businessContext.IsSuperUser = graphQLUserContext.User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == Roles.SuperUser);

            return businessContext;
        }
    }
}
