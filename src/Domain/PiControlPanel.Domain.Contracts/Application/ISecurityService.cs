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
    }
}
