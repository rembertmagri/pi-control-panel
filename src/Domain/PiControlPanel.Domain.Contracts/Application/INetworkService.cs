namespace PiControlPanel.Domain.Contracts.Application
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using PiControlPanel.Domain.Models.Paging;

    /// <summary>
    /// Application layer service for operations on Network model.
    /// </summary>
    public interface INetworkService : IBaseService<Network>
    {
        /// <summary>
        /// Gets the most recent value of the network interface status.
        /// </summary>
        /// <param name="networkInterfaceName">The network interface name.</param>
        /// <returns>The NetworkInterfaceStatus object.</returns>
        Task<NetworkInterfaceStatus> GetLastNetworkInterfaceStatusAsync(string networkInterfaceName);

        /// <summary>
        /// Gets the paged list of values for the network interface status.
        /// </summary>
        /// <param name="networkInterfaceName">The network interface name.</param>
        /// <param name="pagingInput">The paging information.</param>
        /// <returns>The paged info containing the network interface status list.</returns>
        Task<PagingOutput<NetworkInterfaceStatus>> GetNetworkInterfaceStatusesAsync(string networkInterfaceName, PagingInput pagingInput);

        /// <summary>
        /// Gets an observable of the network interface status.
        /// </summary>
        /// <param name="networkInterfaceName">The network interface name.</param>
        /// <returns>The observable NetworkInterfaceStatus.</returns>
        IObservable<NetworkInterfaceStatus> GetNetworkInterfaceStatusObservable(string networkInterfaceName);

        /// <summary>
        /// Retrieves and saves the network interface status.
        /// </summary>
        /// <param name="samplingInterval">The sampling interval in milliseconds to be used to calculate the status.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveNetworkInterfacesStatusAsync(int samplingInterval);
    }
}
