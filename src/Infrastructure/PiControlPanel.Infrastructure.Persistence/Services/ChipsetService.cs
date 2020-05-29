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
        public ChipsetService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.ChipsetRepository;
        }

        public async Task<Chipset> GetAsync(string serial)
        {
            var entity = await this.repository.GetAsync(c => c.Serial == serial);
            return this.mapper.Map<Chipset>(entity);
        }
    }
}
