namespace PiControlPanel.Infrastructure.Persistence.Services.Memory
{
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Memory;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class RandomAccessMemoryStatusService :
        BaseTimedService<RandomAccessMemoryStatus, Entities.Memory.RandomAccessMemoryStatus>,
        IMemoryStatusService<RandomAccessMemoryStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomAccessMemoryStatusService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public RandomAccessMemoryStatusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork.RandomAccessMemoryStatusRepository, unitOfWork, mapper, logger)
        {
        }
    }
}
