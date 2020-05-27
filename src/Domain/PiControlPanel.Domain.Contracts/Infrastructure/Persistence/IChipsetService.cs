namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware;

    public interface IChipsetService : IBaseService<Chipset>
    {
        Task<Chipset> GetAsync(string serial);
    }
}
