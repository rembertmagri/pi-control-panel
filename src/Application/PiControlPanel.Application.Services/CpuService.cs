namespace PiControlPanel.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using PiControlPanel.Domain.Models.Paging;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    /// <inheritdoc/>
    public class CpuService : BaseService<Cpu>, ICpuService
    {
        private readonly Persistence.Cpu.ICpuFrequencyService persistenceFrequencyService;
        private readonly Persistence.Cpu.ICpuTemperatureService persistenceTemperatureService;
        private readonly Persistence.Cpu.ICpuLoadStatusService persistenceLoadStatusService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CpuService"/> class.
        /// </summary>
        /// <param name="persistenceService">The infrastructure layer persistence CPU service.</param>
        /// <param name="persistenceFrequencyService">The infrastructure layer persistence CPU frequency service.</param>
        /// <param name="persistenceTemperatureService">The infrastructure layer persistence CPU temperature service.</param>
        /// <param name="persistenceLoadStatusService">The infrastructure layer persistence CPU load status service.</param>
        /// <param name="onDemandService">The infrastructure layer on demand service.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public CpuService(
            Persistence.Cpu.ICpuService persistenceService,
            Persistence.Cpu.ICpuFrequencyService persistenceFrequencyService,
            Persistence.Cpu.ICpuTemperatureService persistenceTemperatureService,
            Persistence.Cpu.ICpuLoadStatusService persistenceLoadStatusService,
            OnDemand.ICpuService onDemandService,
            ILogger logger)
            : base(persistenceService, onDemandService, logger)
        {
            this.persistenceFrequencyService = persistenceFrequencyService;
            this.persistenceTemperatureService = persistenceTemperatureService;
            this.persistenceLoadStatusService = persistenceLoadStatusService;
        }

        /// <inheritdoc/>
        public Task<CpuLoadStatus> GetLastLoadStatusAsync()
        {
            this.Logger.Debug("Application layer -> CpuService -> GetLastLoadStatusAsync");
            return this.persistenceLoadStatusService.GetLastAsync();
        }

        /// <inheritdoc/>
        public async Task<PagingOutput<CpuLoadStatus>> GetLoadStatusesAsync(PagingInput pagingInput)
        {
            this.Logger.Debug("Application layer -> CpuService -> GetLoadStatusesAsync");
            return await this.persistenceLoadStatusService.GetPageAsync(pagingInput);
        }

        /// <inheritdoc/>
        public IObservable<CpuLoadStatus> GetLoadStatusObservable()
        {
            this.Logger.Debug("Application layer -> CpuService -> GetLoadStatusObservable");
            return ((OnDemand.ICpuService)this.OnDemandService).GetLoadStatusObservable();
        }

        /// <inheritdoc/>
        public async Task<IDictionary<DateTime, double>> GetTotalRealTimeLoadsAsync(
            IEnumerable<DateTime> dateTimes, CancellationToken cancellationToken)
        {
            this.Logger.Debug("Application layer -> CpuService -> GetTotalRealTimeLoad");
            var realTimeLoads = await this.persistenceLoadStatusService.GetCpuLoadStatusesAsync(dateTimes);
            return realTimeLoads.ToDictionary(i => i.Key, i => i.Value.KernelRealTime + i.Value.UserRealTime);
        }

        /// <inheritdoc/>
        public async Task<CpuTemperature> GetLastTemperatureAsync()
        {
            this.Logger.Debug("Application layer -> CpuService -> GetLastTemperatureAsync");
            return await this.persistenceTemperatureService.GetLastAsync();
        }

        /// <inheritdoc/>
        public async Task<PagingOutput<CpuTemperature>> GetTemperaturesAsync(PagingInput pagingInput)
        {
            this.Logger.Debug("Application layer -> CpuService -> GetTemperaturesAsync");
            return await this.persistenceTemperatureService.GetPageAsync(pagingInput);
        }

        /// <inheritdoc/>
        public IObservable<CpuTemperature> GetTemperatureObservable()
        {
            this.Logger.Debug("Application layer -> CpuService -> GetTemperatureObservable");
            return ((OnDemand.ICpuService)this.OnDemandService).GetTemperatureObservable();
        }

        /// <inheritdoc/>
        public async Task<CpuFrequency> GetLastFrequencyAsync()
        {
            this.Logger.Debug("Application layer -> CpuService -> GetLastFrequencyAsync");
            return await this.persistenceFrequencyService.GetLastAsync();
        }

        /// <inheritdoc/>
        public async Task<PagingOutput<CpuFrequency>> GetFrequenciesAsync(PagingInput pagingInput)
        {
            this.Logger.Debug("Application layer -> CpuService -> GetFrequenciesAsync");
            return await this.persistenceFrequencyService.GetPageAsync(pagingInput);
        }

        /// <inheritdoc/>
        public IObservable<CpuFrequency> GetFrequencyObservable()
        {
            this.Logger.Debug("Application layer -> CpuService -> GetFrequencyObservable");
            return ((OnDemand.ICpuService)this.OnDemandService).GetFrequencyObservable();
        }

        /// <inheritdoc/>
        public async Task SaveLoadStatusAsync()
        {
            this.Logger.Debug("Application layer -> CpuService -> SaveLoadStatusAsync");
            var cpu = await this.PersistenceService.GetAsync();
            if (cpu == null)
            {
                this.Logger.Warn("Cpu info not available yet, can't calculate average load");
                return;
            }

            var averageLoad = await ((OnDemand.ICpuService)this.OnDemandService).GetLoadStatusAsync(cpu.Cores);

            await this.persistenceLoadStatusService.AddAsync(averageLoad);
            ((OnDemand.ICpuService)this.OnDemandService).PublishLoadStatus(averageLoad);
        }

        /// <inheritdoc/>
        public async Task SaveTemperatureAsync()
        {
            this.Logger.Debug("Application layer -> CpuService -> SaveTemperatureAsync");
            var temperature = await ((OnDemand.ICpuService)this.OnDemandService).GetTemperatureAsync();

            await this.persistenceTemperatureService.AddAsync(temperature);
            ((OnDemand.ICpuService)this.OnDemandService).PublishTemperature(temperature);
        }

        /// <inheritdoc/>
        public async Task SaveFrequencyAsync(int samplingInterval)
        {
            this.Logger.Debug("Application layer -> CpuService -> SaveFrequencyAsync");
            var frequency = await ((OnDemand.ICpuService)this.OnDemandService).GetFrequencyAsync(samplingInterval);

            await this.persistenceFrequencyService.AddAsync(frequency);
            ((OnDemand.ICpuService)this.OnDemandService).PublishFrequency(frequency);
        }

        /// <inheritdoc/>
        protected async override Task<Cpu> GetPersistedInfoAsync(Cpu onDemandInfo)
        {
            return await ((Persistence.Cpu.ICpuService)this.PersistenceService)
                .GetAsync(onDemandInfo.Model);
        }
    }
}
