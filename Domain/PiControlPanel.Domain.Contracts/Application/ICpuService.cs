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
        Task<CpuAverageLoad> GetLastAverageLoadAsync();

        Task<PagingOutput<CpuAverageLoad>> GetAverageLoadsAsync(PagingInput pagingInput);

        IObservable<CpuAverageLoad> GetAverageLoadObservable();

        Task<CpuRealTimeLoad> GetLastRealTimeLoadAsync();

        Task<PagingOutput<CpuRealTimeLoad>> GetRealTimeLoadsAsync(PagingInput pagingInput);

        IObservable<CpuRealTimeLoad> GetRealTimeLoadObservable();

        Task<IDictionary<DateTime, double>> GetTotalRealTimeLoadsAsync(
            IEnumerable<DateTime> dateTimes, CancellationToken cancellationToken);

        Task<CpuTemperature> GetLastTemperatureAsync();

        Task<PagingOutput<CpuTemperature>> GetTemperaturesAsync(PagingInput pagingInput);

        IObservable<CpuTemperature> GetTemperatureObservable();
        
        Task SaveAverageLoadAsync();

        Task SaveRealTimeLoadAsync();

        Task SaveTemperatureAsync();
    }
}
