namespace PiControlPanel.Infrastructure.Persistence.Services.Cpu
{
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class CpuFrequencyService :
        BaseTimedService<CpuFrequency, Entities.Cpu.CpuFrequency>,
        ICpuFrequencyService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CpuFrequencyService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public CpuFrequencyService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.CpuFrequencyRepository;
        }
    }
}
