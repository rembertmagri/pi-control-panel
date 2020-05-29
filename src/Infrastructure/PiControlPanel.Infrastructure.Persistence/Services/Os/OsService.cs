namespace PiControlPanel.Infrastructure.Persistence.Services.Os
{
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Os;
    using PiControlPanel.Domain.Models.Hardware.Os;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class OsService : BaseService<Os, Entities.Os.Os>, IOsService
    {
        public OsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.OsRepository;
        }
    }
}
