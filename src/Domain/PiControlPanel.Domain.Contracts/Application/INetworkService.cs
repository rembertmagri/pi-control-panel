namespace PiControlPanel.Domain.Contracts.Application
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using PiControlPanel.Domain.Models.Paging;

    public interface INetworkService : IBaseService<Network>
    {
        Task<NetworkInterfaceStatus> GetLastNetworkInterfaceStatusAsync(string networkInterfaceName);

        Task<PagingOutput<NetworkInterfaceStatus>> GetNetworkInterfaceStatusesAsync(string networkInterfaceName, PagingInput pagingInput);

        IObservable<NetworkInterfaceStatus> GetNetworkInterfaceStatusObservable(string networkInterfaceName);

        Task SaveNetworkInterfacesStatusAsync(int samplingInterval);
    }
}
