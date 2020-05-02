namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Network
{
    using PiControlPanel.Domain.Models.Hardware.Network;
    using PiControlPanel.Domain.Models.Paging;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INetworkInterfaceStatusService
    {
        Task<NetworkInterfaceStatus> GetLastAsync(string networkInterfaceName);

        Task<IEnumerable<NetworkInterfaceStatus>> GetAllAsync(string networkInterfaceName);

        Task<PagingOutput<NetworkInterfaceStatus>> GetPageAsync(string networkInterfaceName, PagingInput pagingInput);

        Task AddAsync(NetworkInterfaceStatus model);
    }
}
