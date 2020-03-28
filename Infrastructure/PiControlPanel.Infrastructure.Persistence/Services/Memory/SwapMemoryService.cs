namespace PiControlPanel.Infrastructure.Persistence.Services.Memory
{
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Memory;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    
    public class SwapMemoryService :
        BaseService<SwapMemory, Entities.Memory.SwapMemory>,
        IMemoryService<SwapMemory>
    {
        public SwapMemoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.SwapMemoryRepository;
        }
    }
}
