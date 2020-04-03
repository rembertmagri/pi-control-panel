namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public interface ICpuService : IBaseService<Cpu>
    {
        Task<CpuLoadStatus> GetLoadStatusAsync(int cores);

        IObservable<CpuLoadStatus> GetLoadStatusObservable();

        void PublishLoadStatus(CpuLoadStatus loadStatus);

        Task<CpuTemperature> GetTemperatureAsync();

        IObservable<CpuTemperature> GetTemperatureObservable();

        void PublishTemperature(CpuTemperature temperature);
    }
}
