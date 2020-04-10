namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu
{
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public interface ICpuFrequencyService : IBaseTimedObjectService<CpuFrequency>
    {
    }
}
