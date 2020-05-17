namespace PiControlPanel.Domain.Contracts.Application
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Paging;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using System;

    public interface IMemoryService<T, U> : IBaseService<T>
        where T : Memory
        where U : MemoryStatus
    {
        Task<U> GetLastStatusAsync();

        Task<PagingOutput<U>> GetStatusesAsync(PagingInput pagingInput);

        IObservable<U> GetStatusObservable();

        Task SaveStatusAsync();
    }
}
