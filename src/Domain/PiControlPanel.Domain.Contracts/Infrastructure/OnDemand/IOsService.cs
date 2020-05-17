namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using PiControlPanel.Domain.Models.Hardware.Os;
    using System;
    using System.Threading.Tasks;

    public interface IOsService : IBaseService<Os>
    {
        Task<OsStatus> GetStatusAsync();

        IObservable<OsStatus> GetStatusObservable();

        void PublishStatus(OsStatus status);
    }
}
