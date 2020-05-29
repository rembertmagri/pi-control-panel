namespace PiControlPanel.Domain.Models.Authentication
{
    using System.Collections.Generic;

    /// <summary>
    /// The response sent after the user logs in.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the JWT.
        /// </summary>
        public string JsonWebToken { get; set; }

        /// <summary>
        /// Gets or sets the roles of the user.
        /// </summary>
        public IList<string> Roles { get; set; }
    }
}
