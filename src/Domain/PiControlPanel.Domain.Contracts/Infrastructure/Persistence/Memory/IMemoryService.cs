namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Memory
{
    using PiControlPanel.Domain.Models.Hardware.Memory;

    public interface IMemoryService<T> : IBaseService<T>
        where T : Memory
    {
    }
}
