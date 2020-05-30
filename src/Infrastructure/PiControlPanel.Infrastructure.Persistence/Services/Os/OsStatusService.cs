namespace PiControlPanel.Infrastructure.Persistence.Services.Os
{
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Os;
    using PiControlPanel.Domain.Models.Hardware.Os;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class OsStatusService :
        BaseTimedService<OsStatus, Entities.Os.OsStatus>,
        IOsStatusService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OsStatusService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public OsStatusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.OsStatusRepository;
        }
    }
}
