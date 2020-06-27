namespace PiControlPanel.Domain.Contracts.Constants
{
    /// <summary>
    /// Contains the claim types names.
    /// </summary>
    public static class CustomClaimTypes
    {
        /// <summary>
        /// Username claim type.
        /// </summary>
        public const string Username = nameof(Username);

        /// <summary>
        /// Password claim type.
        /// </summary>
        public const string Password = nameof(Password);

        /// <summary>
        /// Is anonymous claim type.
        /// </summary>
        public const string IsAnonymous = nameof(IsAnonymous);

        /// <summary>
        /// Is authenticated claim type.
        /// </summary>
        public const string IsAuthenticated = nameof(IsAuthenticated);
    }
}
