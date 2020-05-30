namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu
{
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    /// <summary>
    /// Infrastructure layer service for persistence operations on CPU frequency model.
    /// </summary>
    public interface ICpuFrequencyService : IBaseTimedObjectService<CpuFrequency>
    {
    }
}
