namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Network
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using PiControlPanel.Domain.Models.Paging;

    /// <summary>
    /// Infrastructure layer service for persistence operations on network interface status model.
    /// </summary>
    public interface INetworkInterfaceStatusService
    {
        /// <summary>
        /// Gets the most recent value for a specific network interface.
        /// </summary>
        /// <param name="networkInterfaceName">The network interface name.</param>
        /// <returns>A <see cref="Task{NetworkInterfaceStatus}"/> representing the result of the asynchronous operation.</returns>
        Task<NetworkInterfaceStatus> GetLastAsync(string networkInterfaceName);

        /// <summary>
        /// Gets all status of a network interface.
        /// </summary>
        /// <param name="networkInterfaceName">The network interface name.</param>
        /// <returns>A <see cref="Task{NetworkInterfaceStatus}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<NetworkInterfaceStatus>> GetAllAsync(string networkInterfaceName);

        /// <summary>
        /// Gets the paged status of a network interface.
        /// </summary>
        /// <param name="networkInterfaceName">The network interface name.</param>
        /// <param name="pagingInput">The paging information.</param>
        /// <returns>A <see cref="Task{PagingOutput}"/> representing the result of the asynchronous operation.</returns>
        Task<PagingOutput<NetworkInterfaceStatus>> GetPageAsync(string networkInterfaceName, PagingInput pagingInput);

        /// <summary>
        /// Persists multiple values in the database.
        /// </summary>
        /// <param name="networkInterfacesStatus">The network interface status list to be created.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task AddManyAsync(IEnumerable<NetworkInterfaceStatus> networkInterfacesStatus);
    }
}
