namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu
{
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using System.Threading.Tasks;

    public interface ICpuService : IBaseService<Cpu>
    {
        Task<Cpu> GetAsync(string model);
    }
}
