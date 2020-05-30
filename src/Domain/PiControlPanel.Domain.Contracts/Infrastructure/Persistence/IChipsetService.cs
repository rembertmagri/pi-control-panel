namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware;

    /// <summary>
    /// Infrastructure layer service for persistence operations on chipset model.
    /// </summary>
    public interface IChipsetService : IBaseService<Chipset>
    {
        /// <summary>
        /// Gets the chipset model from the database.
        /// </summary>
        /// <param name="serial">The serial number of the chipset to be retrieved.</param>
        /// <returns>A <see cref="Task{Chipset}"/> representing the result of the asynchronous operation.</returns>
        Task<Chipset> GetAsync(string serial);
    }
}
