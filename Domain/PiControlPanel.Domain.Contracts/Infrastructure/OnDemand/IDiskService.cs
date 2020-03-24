namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    public interface IDiskService : IBaseService<Disk>
    {
        Task<DiskStatus> GetStatusAsync();

        IObservable<DiskStatus> GetStatusObservable();

        void PublishStatus(DiskStatus status);
    }
}
