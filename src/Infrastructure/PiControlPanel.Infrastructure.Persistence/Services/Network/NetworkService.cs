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
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public NetworkService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.NetworkRepository;
        }

        /// <inheritdoc/>
        protected override Task<Entities.Network.Network> GetFromRepository()
        {
            return this.repository.GetAsync(s => s.NetworkInterfaces);
        }
    }
}
