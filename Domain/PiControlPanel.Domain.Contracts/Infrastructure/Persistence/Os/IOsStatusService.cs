namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Os
{
    using PiControlPanel.Domain.Models.Hardware.Os;

    public interface IOsStatusService : IBaseTimedObjectService<OsStatus>
    {
    }
}
