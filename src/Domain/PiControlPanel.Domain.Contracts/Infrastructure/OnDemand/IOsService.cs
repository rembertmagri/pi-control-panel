namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Os;

    public interface IOsService : IBaseService<Os>
    {
        Task<OsStatus> GetStatusAsync();

        IObservable<OsStatus> GetStatusObservable();

        void PublishStatus(OsStatus status);
    }
}
