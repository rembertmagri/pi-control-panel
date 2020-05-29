namespace PiControlPanel.Infrastructure.Persistence.Services.Network
{
    using System.Threading.Tasks;
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Network;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class NetworkService : BaseService<Network, Entities.Network.Network>, INetworkService
    {
        public NetworkService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.NetworkRepository;
        }

        protected override Task<Entities.Network.Network> GetFromRepository()
        {
            return this.repository.GetAsync(s => s.NetworkInterfaces);
        }
    }
}
