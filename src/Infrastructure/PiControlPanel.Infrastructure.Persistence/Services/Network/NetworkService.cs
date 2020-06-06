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
        private readonly IRepositoryBase<Entities.Network.NetworkInterface> networkInterfaceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public NetworkService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork.NetworkRepository, unitOfWork, mapper, logger)
        {
            this.networkInterfaceRepository = unitOfWork.NetworkInterfaceRepository;
        }

        /// <inheritdoc/>
        public override async Task UpdateAsync(Network model)
        {
            var entity = this.Mapper.Map<Entities.Network.Network>(model);
            foreach (var networkInterface in entity.NetworkInterfaces)
            {
                var networkInterfaceExists =
                    await this.networkInterfaceRepository.ExistsAsync(n => n.Name == networkInterface.Name);
                if (networkInterfaceExists)
                {
                    this.networkInterfaceRepository.Update(networkInterface);
                }
                else
                {
                    this.networkInterfaceRepository.Create(networkInterface);
                }
            }

            this.Repository.Update(entity);
            await this.UnitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        protected override Task<Entities.Network.Network> GetFromRepository()
        {
            return this.Repository.GetAsync(s => s.NetworkInterfaces);
        }
    }
}
