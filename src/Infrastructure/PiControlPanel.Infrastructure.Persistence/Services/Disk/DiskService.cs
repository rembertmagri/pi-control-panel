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
        private readonly IRepositoryBase<Entities.Disk.FileSystem> fileSystemRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public DiskService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork.DiskRepository, unitOfWork, mapper, logger)
        {
            this.fileSystemRepository = unitOfWork.FileSystemRepository;
        }

        /// <inheritdoc/>
        public override async Task UpdateAsync(Disk model)
        {
            var entity = this.Mapper.Map<Entities.Disk.Disk>(model);
            foreach (var fileSystem in entity.FileSystems)
            {
                var existingFileSystem = await this.fileSystemRepository.GetAsync(f => f.Name == fileSystem.Name);
                if (existingFileSystem == null)
                {
                    this.fileSystemRepository.Create(fileSystem);
                }
                else
                {
                    this.fileSystemRepository.Update(fileSystem);
                }
            }

            this.Repository.Update(entity);
            await this.UnitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        protected override Task<Entities.Disk.Disk> GetFromRepository()
        {
            return this.Repository.GetAsync(s => s.FileSystems);
        }
    }
}
