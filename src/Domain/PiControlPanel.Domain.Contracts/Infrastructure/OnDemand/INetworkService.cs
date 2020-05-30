namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Network;

    /// <summary>
    /// Infrastructure layer service for on demand operations on Network model.
    /// </summary>
    public interface INetworkService : IBaseService<Network>
    {
        /// <summary>
        /// Gets the value of the network interface status for each network interface.
        /// </summary>
        /// <param name="networkInterfaceNames">The list of network interfaces.</param>
        /// <returns>A list of NetworkInterfaceStatus objects.</returns>
        Task<IList<NetworkInterfaceStatus>> GetNetworkInterfacesStatusAsync(IList<string> networkInterfaceNames, int samplingInterval);

        /// <summary>
        /// Gets an observable of the network interface status.
        /// </summary>
        /// <param name="networkInterfaceName">The network interface name.</param>
        /// <returns>The observable NetworkInterfaceStatus.</returns>
        IObservable<NetworkInterfaceStatus> GetNetworkInterfaceStatusObservable(string networkInterfaceName);

        /// <summary>
        /// Publishes the value of the network interfaces status.
        /// </summary>
        /// <param name="networkInterfacesStatus">The values to be publlished.</param>
        void PublishNetworkInterfacesStatus(IList<NetworkInterfaceStatus> networkInterfacesStatus);
    }
}
