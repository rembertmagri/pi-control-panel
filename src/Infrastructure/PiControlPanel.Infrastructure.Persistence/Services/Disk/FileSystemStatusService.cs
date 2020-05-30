namespace PiControlPanel.Infrastructure.Persistence.Services.Disk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Disk;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Paging;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class FileSystemStatusService :
        BaseTimedService<FileSystemStatus, Entities.Disk.FileSystemStatus>,
        IFileSystemStatusService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemStatusService"/> class.
        /// </summary>
        /// <<param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public FileSystemStatusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.FileSystemStatusRepository;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<FileSystemStatus>> GetAllAsync(string fileSystemName)
        {
            Expression<Func<Entities.Disk.FileSystemStatus, bool>> where = e => e.FileSystemName == fileSystemName;
            return this.GetAllAsync(where);
        }

        /// <inheritdoc/>
        public Task<FileSystemStatus> GetLastAsync(string fileSystemName)
        {
            Expression<Func<Entities.Disk.FileSystemStatus, bool>> where = e => e.FileSystemName == fileSystemName;
            return this.GetLastAsync(where);
        }

        /// <inheritdoc/>
        public Task<PagingOutput<FileSystemStatus>> GetPageAsync(string fileSystemName, PagingInput pagingInput)
        {
            Expression<Func<Entities.Disk.FileSystemStatus, bool>> where = e => e.FileSystemName == fileSystemName;
            return this.GetPageAsync(pagingInput, where);
        }

        /// <inheritdoc/>
        public async Task AddManyAsync(IEnumerable<FileSystemStatus> fileSystemsStatus)
        {
            var entities = this.mapper.Map<IEnumerable<Entities.Disk.FileSystemStatus>>(fileSystemsStatus);
            await this.repository.CreateManyAsync(entities.ToArray());
            await this.unitOfWork.CommitAsync();
        }
    }
}
