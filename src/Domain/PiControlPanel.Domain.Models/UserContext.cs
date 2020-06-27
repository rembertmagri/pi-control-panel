namespace PiControlPanel.Domain.Models
{
    /// <summary>
    /// User context containing data required for any business logic flow.
    /// </summary>
    public class UserContext
    {
        /// <summary>
        /// Gets or sets the username of the currently logged in user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the currently logged in user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is anonymous.
        /// </summary>
        public bool IsAnonymous { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is sudoer.
        /// </summary>
        public bool IsSuperUser { get; set; }
    }
}
