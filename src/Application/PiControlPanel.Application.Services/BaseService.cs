namespace PiControlPanel.Application.Services
{
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using System.Threading.Tasks;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

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
            logger.Debug($"Application layer -> BaseService<{typeof(T).Name}> -> GetAsync");
            return persistenceService.GetAsync();
        }

        public async Task SaveAsync()
        {
            logger.Debug($"Application layer -> BaseService<{typeof(T).Name}> -> SaveAsync");
            var onDemandInfo = await this.onDemandService.GetAsync();

            var persistedInfo = await this.GetPersistedInfoAsync(onDemandInfo);
            if (persistedInfo == null)
            {
                logger.Debug($"{typeof(T).Name} info not set on DB, creating...");
                await this.persistenceService.AddAsync(onDemandInfo);
            }
            else
            {
                logger.Debug($"Updating {typeof(T).Name} info on DB...");
                await this.persistenceService.UpdateAsync(onDemandInfo);
            }
        }

        protected virtual async Task<T> GetPersistedInfoAsync(T onDemandInfo)
        {
            return await this.persistenceService.GetAsync();
        }
    }
}
