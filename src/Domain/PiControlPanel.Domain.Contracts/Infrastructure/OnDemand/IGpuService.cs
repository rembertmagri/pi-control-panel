namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using PiControlPanel.Domain.Models.Hardware;

    /// <summary>
    /// Infrastructure layer service for on demand operations on GPU model.
    /// </summary>
    public interface IGpuService : IBaseService<Gpu>
    {
    }
}
