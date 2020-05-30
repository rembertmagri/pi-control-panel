namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence
{
    using PiControlPanel.Domain.Models.Hardware;

    /// <summary>
    /// Infrastructure layer service for persistence operations on GPU model.
    /// </summary>
    public interface IGpuService : IBaseService<Gpu>
    {
    }
}
