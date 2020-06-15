namespace PiControlPanel.Infrastructure.Persistence.Services.Cpu
{
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class CpuSensorsStatusService :
        BaseTimedService<CpuSensorsStatus, Entities.Cpu.CpuSensorsStatus>,
        ICpuSensorsStatusService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CpuSensorsStatusService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public CpuSensorsStatusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork.CpuSensorsStatusRepository, unitOfWork, mapper, logger)
        {
        }
    }
}
