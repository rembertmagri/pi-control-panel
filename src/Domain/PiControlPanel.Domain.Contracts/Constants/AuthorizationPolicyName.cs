namespace PiControlPanel.Domain.Contracts.Constants
{
    /// <summary>
    /// Contains the authorization policy names.
    /// </summary>
    public static class AuthorizationPolicyName
    {
        /// <summary>
        /// Super user policy name.
        /// </summary>
        public const string SuperUserPolicy = nameof(SuperUserPolicy);

        /// <summary>
        /// Authenticated user policy name.
        /// </summary>
        public const string AuthenticatedPolicy = nameof(AuthenticatedPolicy);
    }
}
