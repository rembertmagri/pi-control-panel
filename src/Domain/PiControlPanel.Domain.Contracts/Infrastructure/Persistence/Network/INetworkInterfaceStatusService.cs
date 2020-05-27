namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Network
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using PiControlPanel.Domain.Models.Paging;

    public interface INetworkInterfaceStatusService
    {
        Task<NetworkInterfaceStatus> GetLastAsync(string networkInterfaceName);

        Task<IEnumerable<NetworkInterfaceStatus>> GetAllAsync(string networkInterfaceName);

        Task<PagingOutput<NetworkInterfaceStatus>> GetPageAsync(string networkInterfaceName, PagingInput pagingInput);

        Task AddManyAsync(IEnumerable<NetworkInterfaceStatus> networkInterfacesStatus);
    }
}
