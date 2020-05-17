namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    public interface IMemoryService<T, U> : IBaseService<T>
        where T : Memory
        where U : MemoryStatus
    {
        Task<U> GetStatusAsync();

        IObservable<U> GetStatusObservable();

        void PublishStatus(U status);
    }
}
