namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public interface ICpuService : IBaseService<Cpu>
    {
        Task<CpuAverageLoad> GetAverageLoadAsync(int cores);

        IObservable<CpuAverageLoad> GetAverageLoadObservable();

        void PublishAverageLoad(CpuAverageLoad averageLoad);

        Task<CpuRealTimeLoad> GetRealTimeLoadAsync();

        IObservable<CpuRealTimeLoad> GetRealTimeLoadObservable();

        void PublishRealTimeLoad(CpuRealTimeLoad realTimeLoad);

        Task<CpuTemperature> GetTemperatureAsync();

        IObservable<CpuTemperature> GetTemperatureObservable();

        void PublishTemperature(CpuTemperature temperature);
    }
}
