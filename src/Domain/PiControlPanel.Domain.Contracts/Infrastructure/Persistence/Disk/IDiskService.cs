namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Disk
{
    using PiControlPanel.Domain.Models.Hardware.Disk;

    /// <summary>
    /// Infrastructure layer service for persistence operations on disk model.
    /// </summary>
    public interface IDiskService : IBaseService<Disk>
    {
    }
}
