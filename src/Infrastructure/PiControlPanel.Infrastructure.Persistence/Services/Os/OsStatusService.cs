namespace PiControlPanel.Infrastructure.Persistence.Services.Os
{
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Os;
    using PiControlPanel.Domain.Models.Hardware.Os;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    
    public class OsStatusService :
        BaseTimedService<OsStatus, Entities.Os.OsStatus>,
        IOsStatusService
    {
        public OsStatusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.OsStatusRepository;
        }
    }
}
