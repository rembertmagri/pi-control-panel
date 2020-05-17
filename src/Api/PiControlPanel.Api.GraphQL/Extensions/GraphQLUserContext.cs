namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System.Security.Claims;
    using global::GraphQL.Authorization;

    /// <summary>
    /// The GraphQL user context for the current request. The user context is accessible in field resolvers and
    /// validation rules using <c>context.UserContext.As&lt;GraphQLUserContext&gt;()</c>.
    /// </summary>
    public class GraphQLUserContext : IProvideClaimsPrincipal
    {
        /// <summary>
        /// Gets the current users claims principal.
        /// </summary>
        public ClaimsPrincipal User { get; set; }
    }
}
