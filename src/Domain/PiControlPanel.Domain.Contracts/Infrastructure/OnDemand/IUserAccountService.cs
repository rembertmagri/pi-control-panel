namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Authentication;

    /// <summary>
    /// Infrastructure layer service for on demand authorization and authentication.
    /// </summary>
    public interface IUserAccountService
    {
        /// <summary>
        /// Checks if a username/password combination is valid.
        /// </summary>
        /// <param name="userAccount">The user account information.</param>
        /// <returns>Whether the user account information is valid.</returns>
        Task<bool> ValidateAsync(UserAccount userAccount);

        /// <summary>
        /// Checks if a user is sudoer.
        /// </summary>
        /// <param name="userAccount">The user account information.</param>
        /// <returns>Whether the user is super user.</returns>
        Task<bool> IsSuperUserAsync(UserAccount userAccount);
    }
}
