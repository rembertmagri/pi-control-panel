namespace PiControlPanel.Application.Services
{
    using NLog;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using PiControlPanel.Domain.Models.Paging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OnDemand = PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;
    using Persistence = PiControlPanel.Domain.Contracts.Infrastructure.Persistence;

    public class CpuService : BaseService<Cpu>, ICpuService
    {
        private readonly Persistence.Cpu.ICpuFrequencyService persistenceFrequencyService;
        private readonly Persistence.Cpu.ICpuTemperatureService persistenceTemperatureService;
        private readonly Persistence.Cpu.ICpuLoadStatusService persistenceLoadStatusService;

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

        public Task<CpuLoadStatus> GetLastLoadStatusAsync()
        {
            logger.Trace("Application layer -> CpuService -> GetLastLoadStatusAsync");
            return this.persistenceLoadStatusService.GetLastAsync();
        }

        public async Task<PagingOutput<CpuLoadStatus>> GetLoadStatusesAsync(PagingInput pagingInput)
        {
            logger.Trace("Application layer -> CpuService -> GetLoadStatusesAsync");
            return await this.persistenceLoadStatusService.GetPageAsync(pagingInput);
        }

        public IObservable<CpuLoadStatus> GetLoadStatusObservable()
        {
            logger.Trace("Application layer -> CpuService -> GetLoadStatusObservable");
            return ((OnDemand.ICpuService)this.onDemandService).GetLoadStatusObservable();
        }

        public async Task<IDictionary<DateTime, double>> GetTotalRealTimeLoadsAsync(
            IEnumerable<DateTime> dateTimes, CancellationToken cancellationToken)
        {
            logger.Trace("Application layer -> CpuService -> GetTotalRealTimeLoad");
            var realTimeLoads = await this.persistenceLoadStatusService.GetCpuLoadStatusesAsync(dateTimes);
            return realTimeLoads.ToDictionary(i => i.Key, i => i.Value.KernelRealTime + i.Value.UserRealTime);
        }

        public async Task<CpuTemperature> GetLastTemperatureAsync()
        {
            logger.Trace("Application layer -> CpuService -> GetLastTemperatureAsync");
            return await this.persistenceTemperatureService.GetLastAsync();
        }

        public async Task<PagingOutput<CpuTemperature>> GetTemperaturesAsync(PagingInput pagingInput)
        {
            logger.Trace("Application layer -> CpuService -> GetTemperaturesAsync");
            return await this.persistenceTemperatureService.GetPageAsync(pagingInput);
        }

        public IObservable<CpuTemperature> GetTemperatureObservable()
        {
            logger.Trace("Application layer -> CpuService -> GetTemperatureObservable");
            return ((OnDemand.ICpuService)this.onDemandService).GetTemperatureObservable();
        }

        public async Task<CpuFrequency> GetLastFrequencyAsync()
        {
            logger.Trace("Application layer -> CpuService -> GetLastFrequencyAsync");
            return await this.persistenceFrequencyService.GetLastAsync();
        }

        public async Task<PagingOutput<CpuFrequency>> GetFrequenciesAsync(PagingInput pagingInput)
        {
            logger.Trace("Application layer -> CpuService -> GetFrequenciesAsync");
            return await this.persistenceFrequencyService.GetPageAsync(pagingInput);
        }

        public IObservable<CpuFrequency> GetFrequencyObservable()
        {
            logger.Trace("Application layer -> CpuService -> GetFrequencyObservable");
            return ((OnDemand.ICpuService)this.onDemandService).GetFrequencyObservable();
        }

        public async Task SaveLoadStatusAsync()
        {
            logger.Trace("Application layer -> CpuService -> SaveLoadStatusAsync");
            var cpu = await this.persistenceService.GetAsync();
            if (cpu == null)
            {
                logger.Warn("Cpu info not available yet, can't calculate average load");
                return;
            }

            var averageLoad = await ((OnDemand.ICpuService)this.onDemandService).GetLoadStatusAsync(cpu.Cores);

            await this.persistenceLoadStatusService.AddAsync(averageLoad);
            ((OnDemand.ICpuService)this.onDemandService).PublishLoadStatus(averageLoad);
        }

        public async Task SaveTemperatureAsync()
        {
            logger.Trace("Application layer -> CpuService -> SaveTemperatureAsync");
            var temperature = await ((OnDemand.ICpuService)this.onDemandService).GetTemperatureAsync();

            await this.persistenceTemperatureService.AddAsync(temperature);
            ((OnDemand.ICpuService)this.onDemandService).PublishTemperature(temperature);
        }

        public async Task SaveFrequencyAsync(int samplingInterval)
        {
            logger.Trace("Application layer -> CpuService -> SaveFrequencyAsync");
            var frequency = await ((OnDemand.ICpuService)this.onDemandService).GetFrequencyAsync(samplingInterval);

            await this.persistenceFrequencyService.AddAsync(frequency);
            ((OnDemand.ICpuService)this.onDemandService).PublishFrequency(frequency);
        }

        protected async override Task<Cpu> GetPersistedInfoAsync(Cpu onDemandInfo)
        {
            return await ((Persistence.Cpu.ICpuService)this.persistenceService)
                .GetAsync(onDemandInfo.Model);
        }
    }
}
