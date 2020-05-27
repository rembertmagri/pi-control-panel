namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    public interface ICpuService : IBaseService<Cpu>
    {
        Task<Cpu> GetAsync(string model);
    }
}
