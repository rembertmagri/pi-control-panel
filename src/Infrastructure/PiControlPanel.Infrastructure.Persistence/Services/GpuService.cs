namespace PiControlPanel.Infrastructure.Persistence.Services
{
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence;
    using PiControlPanel.Domain.Models.Hardware;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    
    public class GpuService : BaseService<Gpu, Entities.Gpu>, IGpuService
    {
        public GpuService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.GpuRepository;
        }
    }
}
