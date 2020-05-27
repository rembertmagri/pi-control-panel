namespace PiControlPanel.Infrastructure.Persistence.Services.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Network;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using PiControlPanel.Domain.Models.Paging;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    public class NetworkInterfaceStatusService :
        BaseTimedService<NetworkInterfaceStatus, Entities.Network.NetworkInterfaceStatus>,
        INetworkInterfaceStatusService
    {
        public NetworkInterfaceStatusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.NetworkInterfaceStatusRepository;
        }

        public Task<IEnumerable<NetworkInterfaceStatus>> GetAllAsync(string networkInterfaceName)
        {
            Expression<Func<Entities.Network.NetworkInterfaceStatus, bool>> where = (e => e.NetworkInterfaceName == networkInterfaceName);
            return this.GetAllAsync(where);
        }

        public Task<NetworkInterfaceStatus> GetLastAsync(string networkInterfaceName)
        {
            Expression<Func<Entities.Network.NetworkInterfaceStatus, bool>> where = (e => e.NetworkInterfaceName == networkInterfaceName);
            return this.GetLastAsync(where);
        }

        public Task<PagingOutput<NetworkInterfaceStatus>> GetPageAsync(string networkInterfaceName, PagingInput pagingInput)
        {
            Expression<Func<Entities.Network.NetworkInterfaceStatus, bool>> where = (e => e.NetworkInterfaceName == networkInterfaceName);
            return this.GetPageAsync(pagingInput, where);
        }

        public async Task AddManyAsync(IEnumerable<NetworkInterfaceStatus> networkInterfacesStatus)
        {
            var entities = this.mapper.Map<IEnumerable<Entities.Network.NetworkInterfaceStatus>>(networkInterfacesStatus);
            await this.repository.CreateManyAsync(entities.ToArray());
            await this.unitOfWork.CommitAsync();
        }
    }
}
