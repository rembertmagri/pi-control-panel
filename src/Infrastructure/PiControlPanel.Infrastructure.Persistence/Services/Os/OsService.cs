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
        /// <summary>
        /// Initializes a new instance of the <see cref="OsService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public OsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork.OsRepository, unitOfWork, mapper, logger)
        {
        }
    }
}
