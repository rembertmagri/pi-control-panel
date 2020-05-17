namespace PiControlPanel.Domain.Contracts.Application
{
    using PiControlPanel.Domain.Models.Hardware.Os;
    using PiControlPanel.Domain.Models.Paging;
    using System;
    using System.Threading.Tasks;

    public interface IOsService : IBaseService<Os>
    {
        Task<OsStatus> GetLastStatusAsync();

        Task<PagingOutput<OsStatus>> GetStatusesAsync(PagingInput pagingInput);

        IObservable<OsStatus> GetStatusObservable();

        Task SaveStatusAsync();
    }
}
