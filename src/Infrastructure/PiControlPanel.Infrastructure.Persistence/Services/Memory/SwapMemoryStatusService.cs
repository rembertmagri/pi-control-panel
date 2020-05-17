namespace PiControlPanel.Infrastructure.Persistence.Services.Memory
{
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Memory;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    
    public class SwapMemoryStatusService :
        BaseTimedService<SwapMemoryStatus, Entities.Memory.SwapMemoryStatus>,
        IMemoryStatusService<SwapMemoryStatus>
    {
        public SwapMemoryStatusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.SwapMemoryStatusRepository;
        }
    }
}
