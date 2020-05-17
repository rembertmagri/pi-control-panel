namespace PiControlPanel.Infrastructure.Persistence.Services.Cpu
{
    using System.Threading.Tasks;
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    
    public class CpuService : BaseService<Cpu, Entities.Cpu.Cpu>, ICpuService
    {
        public CpuService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.CpuRepository;
        }

        public async Task<Cpu> GetAsync(string model)
        {
            var entity = await repository.GetAsync(c => c.Model == model);
            return mapper.Map<Cpu>(entity);
        }
    }
}
