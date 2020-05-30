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

    /// <inheritdoc/>
    public class NetworkInterfaceStatusService :
        BaseTimedService<NetworkInterfaceStatus, Entities.Network.NetworkInterfaceStatus>,
        INetworkInterfaceStatusService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkInterfaceStatusService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public NetworkInterfaceStatusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork.NetworkInterfaceStatusRepository, unitOfWork, mapper, logger)
        {
        }

        /// <inheritdoc/>
        public Task<IEnumerable<NetworkInterfaceStatus>> GetAllAsync(string networkInterfaceName)
        {
            Expression<Func<Entities.Network.NetworkInterfaceStatus, bool>> where = e => e.NetworkInterfaceName == networkInterfaceName;
            return this.GetAllAsync(where);
        }

        /// <inheritdoc/>
        public Task<NetworkInterfaceStatus> GetLastAsync(string networkInterfaceName)
        {
            Expression<Func<Entities.Network.NetworkInterfaceStatus, bool>> where = e => e.NetworkInterfaceName == networkInterfaceName;
            return this.GetLastAsync(where);
        }

        /// <inheritdoc/>
        public Task<PagingOutput<NetworkInterfaceStatus>> GetPageAsync(string networkInterfaceName, PagingInput pagingInput)
        {
            Expression<Func<Entities.Network.NetworkInterfaceStatus, bool>> where = e => e.NetworkInterfaceName == networkInterfaceName;
            return this.GetPageAsync(pagingInput, where);
        }

        /// <inheritdoc/>
        public async Task AddManyAsync(IEnumerable<NetworkInterfaceStatus> networkInterfacesStatus)
        {
            var entities = this.Mapper.Map<IEnumerable<Entities.Network.NetworkInterfaceStatus>>(networkInterfacesStatus);
            await this.Repository.CreateManyAsync(entities.ToArray());
            await this.UnitOfWork.CommitAsync();
        }
    }
}
