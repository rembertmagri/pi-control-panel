namespace PiControlPanel.Domain.Contracts.Application
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Os;
    using PiControlPanel.Domain.Models.Paging;

    public interface IOsService : IBaseService<Os>
    {
        Task<OsStatus> GetLastStatusAsync();

        Task<PagingOutput<OsStatus>> GetStatusesAsync(PagingInput pagingInput);

        IObservable<OsStatus> GetStatusObservable();

        Task SaveStatusAsync();
    }
}
