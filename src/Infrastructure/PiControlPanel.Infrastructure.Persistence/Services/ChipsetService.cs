namespace PiControlPanel.Infrastructure.Persistence.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence;
    using PiControlPanel.Domain.Models.Hardware;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class ChipsetService : BaseService<Chipset, Entities.Chipset>, IChipsetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChipsetService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public ChipsetService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.ChipsetRepository;
        }

        /// <inheritdoc/>
        public async Task<Chipset> GetAsync(string serial)
        {
            var entity = await this.repository.GetAsync(c => c.Serial == serial);
            return this.mapper.Map<Chipset>(entity);
        }
    }
}
