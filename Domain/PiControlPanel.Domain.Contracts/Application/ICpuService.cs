namespace PiControlPanel.Domain.Contracts.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using PiControlPanel.Domain.Models.Paging;

    public interface ICpuService : IBaseService<Cpu>
    {
        Task<CpuLoadStatus> GetLastLoadStatusAsync();

        Task<PagingOutput<CpuLoadStatus>> GetLoadStatusesAsync(PagingInput pagingInput);

        IObservable<CpuLoadStatus> GetLoadStatusObservable();

        Task<IDictionary<DateTime, double>> GetTotalRealTimeLoadsAsync(
            IEnumerable<DateTime> dateTimes, CancellationToken cancellationToken);

        Task<CpuTemperature> GetLastTemperatureAsync();

        Task<PagingOutput<CpuTemperature>> GetTemperaturesAsync(PagingInput pagingInput);

        IObservable<CpuTemperature> GetTemperatureObservable();
        
        Task SaveLoadStatusAsync();

        Task SaveTemperatureAsync();
    }
}
