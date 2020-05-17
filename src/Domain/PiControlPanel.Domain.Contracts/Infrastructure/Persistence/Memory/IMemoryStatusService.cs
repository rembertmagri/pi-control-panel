namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Memory
{
    using PiControlPanel.Domain.Models.Hardware.Memory;

    public interface IMemoryStatusService<T> : IBaseTimedObjectService<T>
        where T : MemoryStatus
    {
    }
}
