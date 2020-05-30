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
        /// <summary>
        /// Initializes a new instance of the <see cref="DiskService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public DiskService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork.DiskRepository, unitOfWork, mapper, logger)
        {
        }

        /// <inheritdoc/>
        protected override Task<Entities.Disk.Disk> GetFromRepository()
        {
            return this.Repository.GetAsync(s => s.FileSystems);
        }
    }
}
