namespace PiControlPanel.Api.GraphQL.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using global::GraphQL.Authorization;

    /// <inheritdoc/>
    [Serializable]
    public sealed class GraphQLUserContext : Dictionary<string, object>, IProvideClaimsPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphQLUserContext"/> class.
        /// </summary>
        /// <param name="claims">The current users claims principal from the token.</param>
        public GraphQLUserContext(ClaimsPrincipal claims)
            : base()
        {
            this.User = claims;
            this["UserContext"] = claims.GetUserContext();
        }

        /// <summary>
        /// Gets the current users claims principal.
        /// </summary>
        public ClaimsPrincipal User { get; }
    }
}
