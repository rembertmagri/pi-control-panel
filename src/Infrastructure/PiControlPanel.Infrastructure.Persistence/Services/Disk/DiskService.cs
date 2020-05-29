namespace PiControlPanel.Infrastructure.Persistence.Services.Disk
{
    using System.Threading.Tasks;
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Disk;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class DiskService : BaseService<Disk, Entities.Disk.Disk>, IDiskService
    {
        public DiskService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.DiskRepository;
        }

        protected override Task<Entities.Disk.Disk> GetFromRepository()
        {
            return this.repository.GetAsync(s => s.FileSystems);
        }
    }
}
