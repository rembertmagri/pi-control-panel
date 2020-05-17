namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence
{
    using PiControlPanel.Domain.Models.Hardware;
    using System.Threading.Tasks;

    public interface IChipsetService : IBaseService<Chipset>
    {
        Task<Chipset> GetAsync(string serial);
    }
}
