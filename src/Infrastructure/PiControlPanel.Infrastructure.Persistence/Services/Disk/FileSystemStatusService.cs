namespace PiControlPanel.Infrastructure.Persistence.Services.Disk
{
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Disk;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Paging;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class FileSystemStatusService :
        BaseTimedService<FileSystemStatus, Entities.Disk.FileSystemStatus>,
        IFileSystemStatusService
    {
        public FileSystemStatusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.FileSystemStatusRepository;
        }

        public Task<IEnumerable<FileSystemStatus>> GetAllAsync(string fileSystemName)
        {
            Expression<Func<Entities.Disk.FileSystemStatus, bool>> where = (e => e.FileSystemName == fileSystemName);
            return base.GetAllAsync(where);
        }

        public Task<FileSystemStatus> GetLastAsync(string fileSystemName)
        {
            Expression<Func<Entities.Disk.FileSystemStatus, bool>> where = (e => e.FileSystemName == fileSystemName);
            return base.GetLastAsync(where);
        }

        public Task<PagingOutput<FileSystemStatus>> GetPageAsync(string fileSystemName, PagingInput pagingInput)
        {
            Expression<Func<Entities.Disk.FileSystemStatus, bool>> where = (e => e.FileSystemName == fileSystemName);
            return base.GetPageAsync(pagingInput, where);
        }

        public async Task AddManyAsync(IEnumerable<FileSystemStatus> fileSystemsStatus)
        {
            var entities = mapper.Map<IEnumerable<Entities.Disk.FileSystemStatus>>(fileSystemsStatus);
            await repository.CreateManyAsync(entities.ToArray());
            await this.unitOfWork.CommitAsync();
        }
    }
}
