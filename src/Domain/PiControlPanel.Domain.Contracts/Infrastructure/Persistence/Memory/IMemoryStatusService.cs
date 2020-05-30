namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Memory
{
    using PiControlPanel.Domain.Models.Hardware.Memory;

    /// <summary>
    /// Infrastructure layer service for persistence operations on memory status model.
    /// </summary>
    /// <typeparam name="T">The memory status generic type parameter.</typeparam>
    public interface IMemoryStatusService<T> : IBaseTimedObjectService<T>
        where T : MemoryStatus
    {
    }
}
