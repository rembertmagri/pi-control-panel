namespace PiControlPanel.Domain.Contracts.Application
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Paging;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using System;

    public interface IMemoryService : IBaseService<Memory>
    {
        Task<MemoryStatus> GetLastStatusAsync();

        Task<PagingOutput<MemoryStatus>> GetStatusesAsync(PagingInput pagingInput);

        IObservable<MemoryStatus> GetStatusObservable();

        Task SaveStatusAsync();
    }
}
