namespace PiControlPanel.Domain.Contracts.Application
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Authentication;

    /// <summary>
    /// Application layer service for authenticating.
    /// </summary>
    public interface ISecurityService
    {
        /// <summary>
        /// Logs the user in if valid, returning the LoginResponse object.
        /// </summary>
        /// <param name="userAccount">The user account information.</param>
        /// <returns>The LoginResponse object.</returns>
        Task<LoginResponse> LoginAsync(UserAccount userAccount);

        /// <summary>
        /// Refreshes the LoginResponse object.
        /// </summary>
        /// <param name="userAccount">The user account information.</param>
        /// <returns>The LoginResponse object.</returns>
        Task<LoginResponse> GetLoginResponseAsync(UserAccount userAccount);

        /// <summary>
        /// Recovers the username and password of the token issuee.
        /// </summary>
        /// <param name="token">The JWT string.</param>
        /// <returns>The UserAccount object.</returns>
        Task<UserAccount> GetUserAccountAsync(string token);
    }
}
