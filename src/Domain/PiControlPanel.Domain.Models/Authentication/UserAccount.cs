namespace PiControlPanel.Domain.Models.Authentication
{
    /// <summary>
    /// The user account information.
    /// </summary>
    public class UserAccount
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }
    }
}
