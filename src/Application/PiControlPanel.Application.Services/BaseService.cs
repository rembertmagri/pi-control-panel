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
        protected readonly Persistence.IBaseService<T> persistenceService;
        protected readonly OnDemand.IBaseService<T> onDemandService;
        protected readonly ILogger logger;

        public BaseService(
            Persistence.IBaseService<T> persistenceService,
            OnDemand.IBaseService<T> onDemandService,
            ILogger logger)
        {
            this.persistenceService = persistenceService;
            this.onDemandService = onDemandService;
            this.logger = logger;
        }

        public Task<T> GetAsync()
        {
            this.logger.Debug($"Application layer -> BaseService<{typeof(T).Name}> -> GetAsync");
            return this.persistenceService.GetAsync();
        }

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

        protected virtual async Task<T> GetPersistedInfoAsync(T onDemandInfo)
        {
            return await this.persistenceService.GetAsync();
        }
    }
}
