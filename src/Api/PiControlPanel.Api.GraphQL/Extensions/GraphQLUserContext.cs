namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.Security.Claims;
    using global::GraphQL.Authorization;

    /// <inheritdoc/>
    public class GraphQLUserContext : IProvideClaimsPrincipal
    {
        /// <summary>
        /// Gets or sets the current users claims principal.
        /// </summary>
        public ClaimsPrincipal User { get; set; }
    }
}
