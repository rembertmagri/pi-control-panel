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
            this.PersistenceService = persistenceService;
            this.OnDemandService = onDemandService;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the infrastructure layer persistence service.
        /// </summary>
        protected Persistence.IBaseService<T> PersistenceService { get; }

        /// <summary>
        /// Gets the infrastructure layer on demand service.
        /// </summary>
        protected OnDemand.IBaseService<T> OnDemandService { get; }

        /// <summary>
        /// Gets the NLog logger instance.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc/>
        public Task<T> GetAsync()
        {
            this.Logger.Debug($"Application layer -> BaseService<{typeof(T).Name}> -> GetAsync");
            return this.PersistenceService.GetAsync();
        }

        /// <inheritdoc/>
        public async Task SaveAsync()
        {
            this.Logger.Debug($"Application layer -> BaseService<{typeof(T).Name}> -> SaveAsync");
            var onDemandInfo = await this.OnDemandService.GetAsync();

            var persistedInfo = await this.GetPersistedInfoAsync(onDemandInfo);
            if (persistedInfo == null)
            {
                this.Logger.Debug($"{typeof(T).Name} info not set on DB, creating...");
                await this.PersistenceService.AddAsync(onDemandInfo);
            }
            else
            {
                this.Logger.Debug($"Updating {typeof(T).Name} info on DB...");
                await this.PersistenceService.UpdateAsync(onDemandInfo);
            }
        }

        /// <summary>
        /// Gets the already persisted information from the database.
        /// </summary>
        /// <param name="onDemandInfo">The information got from the on demand service.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        protected virtual async Task<T> GetPersistedInfoAsync(T onDemandInfo)
        {
            return await this.PersistenceService.GetAsync();
        }
    }
}
