namespace PiControlPanel.Infrastructure.Persistence.Services.Cpu
{
    using System.Threading.Tasks;
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class CpuService : BaseService<Cpu, Entities.Cpu.Cpu>, ICpuService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CpuService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public CpuService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork.CpuRepository, unitOfWork, mapper, logger)
        {
        }

        /// <inheritdoc/>
        public async Task<Cpu> GetAsync(string model)
        {
            var entity = await this.Repository.GetAsync(c => c.Model == model);
            return this.Mapper.Map<Cpu>(entity);
        }
    }
}
