namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models;
    using PiControlPanel.Domain.Models.Enums;

    /// <summary>
    /// Infrastructure layer service for performing on demand actions on Raspberry Pi.
    /// </summary>
    public interface IControlPanelService
    {
        /// <summary>
        /// Reboots the board.
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        Task<bool> RebootAsync();

        /// <summary>
        /// Shutdown the board.
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        Task<bool> ShutdownAsync();

        /// <summary>
        /// Updates the firmware of the board.
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        Task<bool> UpdateAsync();

        /// <summary>
        /// Kills a specific process.
        /// </summary>
        /// <param name="context">The user context.</param>
        /// <param name="processId">The process identifier.</param>
        /// <returns>Whether the operation was successful.</returns>
        Task<bool> KillAsync(UserContext context, int processId);

        /// <summary>
        /// Gets the username of the owner of a process.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <returns>The process owner username.</returns>
        Task<string> GetProcessOwnerUsernameAsync(int processId);

        /// <summary>
        /// Changes the clock configuration of the board.
        /// </summary>
        /// <param name="cpuMaxFrequencyLevel">The new CPU maximum frequency level.</param>
        /// <returns>Whether the operation was successful.</returns>
        Task<bool> OverclockAsync(CpuMaxFrequencyLevel cpuMaxFrequencyLevel);

        /// <summary>
        /// Starts the SSH server service.
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        Task<bool> StartSshAsync();
    }
}
