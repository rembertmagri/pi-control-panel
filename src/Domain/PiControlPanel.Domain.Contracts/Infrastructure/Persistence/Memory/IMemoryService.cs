namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Memory
{
    using PiControlPanel.Domain.Models.Hardware.Memory;

    /// <summary>
    /// Infrastructure layer service for persistence operations on memory model.
    /// </summary>
    /// <typeparam name="T">The Memory generic type parameter.</typeparam>
    public interface IMemoryService<T> : IBaseService<T>
        where T : Memory
    {
    }
}
