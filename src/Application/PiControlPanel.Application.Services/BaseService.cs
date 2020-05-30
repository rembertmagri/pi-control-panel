namespace PiControlPanel.Application.Services
{
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    /// <inheritdoc/>
    public abstract class BaseService<T> : IBaseService<T>
    {
        /// <summary>
        /// The infrastructure layer persistence service.
        /// </summary>
        protected readonly Persistence.IBaseService<T> persistenceService;

        /// <summary>
        /// The infrastructure layer on demand service.
        /// </summary>
        protected readonly OnDemand.IBaseService<T> onDemandService;

        /// <summary>
        /// The NLog logger instance.
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// </summary>
        /// <param name="persistenceService">The infrastructure layer persistence service.</param>
        /// <param name="onDemandService">The infrastructure layer on demand service.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public BaseService(
            Persistence.IBaseService<T> persistenceService,
            OnDemand.IBaseService<T> onDemandService,
            ILogger logger)
        {
            this.persistenceService = persistenceService;
            this.onDemandService = onDemandService;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Task<T> GetAsync()
        {
            this.logger.Debug($"Application layer -> BaseService<{typeof(T).Name}> -> GetAsync");
            return this.persistenceService.GetAsync();
        }

        /// <inheritdoc/>
        public async Task SaveAsync()
        {
            this.logger.Debug($"Application layer -> BaseService<{typeof(T).Name}> -> SaveAsync");
            var onDemandInfo = await this.onDemandService.GetAsync();

            var persistedInfo = await this.GetPersistedInfoAsync(onDemandInfo);
            if (persistedInfo == null)
            {
                this.logger.Debug($"{typeof(T).Name} info not set on DB, creating...");
                await this.persistenceService.AddAsync(onDemandInfo);
            }
            else
            {
                this.logger.Debug($"Updating {typeof(T).Name} info on DB...");
                await this.persistenceService.UpdateAsync(onDemandInfo);
            }
        }

        /// <summary>
        /// Gets the already persisted information from the database.
        /// </summary>
        /// <param name="onDemandInfo">The information got from the on demand service.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        protected virtual async Task<T> GetPersistedInfoAsync(T onDemandInfo)
        {
            return await this.persistenceService.GetAsync();
        }
    }
}
