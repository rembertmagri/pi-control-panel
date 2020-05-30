namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Os
{
    using PiControlPanel.Domain.Models.Hardware.Os;

    /// <summary>
    /// Infrastructure layer service for persistence operations on operating system status model.
    /// </summary>
    public interface IOsStatusService : IBaseTimedObjectService<OsStatus>
    {
    }
}
