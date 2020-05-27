namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Network;

    public interface INetworkService : IBaseService<Network>
    {
        Task<IList<NetworkInterfaceStatus>> GetNetworkInterfacesStatusAsync(IList<string> networkInterfaceNames, int samplingInterval);

        IObservable<NetworkInterfaceStatus> GetNetworkInterfaceStatusObservable(string networkInterfaceName);

        void PublishNetworkInterfacesStatus(IList<NetworkInterfaceStatus> networkInterfacesStatus);
    }
}
