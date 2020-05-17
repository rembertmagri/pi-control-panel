namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using PiControlPanel.Domain.Models.Hardware.Network;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INetworkService : IBaseService<Network>
    {
        Task<IList<NetworkInterfaceStatus>> GetNetworkInterfacesStatusAsync(IList<string> networkInterfaceNames, int samplingInterval);

        IObservable<NetworkInterfaceStatus> GetNetworkInterfaceStatusObservable(string networkInterfaceName);

        void PublishNetworkInterfacesStatus(IList<NetworkInterfaceStatus> networkInterfacesStatus);
    }
}
