namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence.Cpu
{
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware.Cpu;

    /// <summary>
    /// Infrastructure layer service for persistence operations on CPU model.
    /// </summary>
    public interface ICpuService : IBaseService<Cpu>
    {
        /// <summary>
        /// Gets the value from the database.
        /// </summary>
        /// <param name="model">The model of the CPU.</param>
        /// <returns>A <see cref="Task{Cpu}"/> representing the result of the asynchronous operation.</returns>
        Task<Cpu> GetAsync(string model);
    }
}
