namespace PiControlPanel.Domain.Contracts.Application
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models;
    using PiControlPanel.Domain.Models.Enums;

    /// <summary>
    /// Application layer service for performing actions on Raspberry Pi.
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
        /// <param name="context">The business context.</param>
        /// <param name="processId">The process identifier.</param>
        /// <returns>Whether the operation was successful.</returns>
        Task<bool> KillAsync(BusinessContext context, int processId);

        /// <summary>
        /// Checks is the logged in user can kill a process.
        /// </summary>
        /// <param name="context">The business context.</param>
        /// <param name="processId">The process identifier.</param>
        /// <returns>Whether the user can kill the process.</returns>
        Task<bool> IsAuthorizedToKillAsync(BusinessContext context, int processId);

        /// <summary>
        /// Changes the clock configuration of the board.
        /// </summary>
        /// <param name="cpuMaxFrequencyLevel">The new CPU maximum frequency level.</param>
        /// <returns>Whether the operation was successful.</returns>
        Task<bool> OverclockAsync(CpuMaxFrequencyLevel cpuMaxFrequencyLevel);
    }
}
