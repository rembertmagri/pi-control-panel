namespace PiControlPanel.Infrastructure.Persistence.Services.Memory
{
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Memory;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;

    /// <inheritdoc/>
    public class RandomAccessMemoryService :
        BaseService<RandomAccessMemory, Entities.Memory.RandomAccessMemory>,
        IMemoryService<RandomAccessMemory>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomAccessMemoryService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public RandomAccessMemoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
            this.repository = unitOfWork.RandomAccessMemoryRepository;
        }
    }
}
