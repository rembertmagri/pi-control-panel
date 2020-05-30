namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System.Threading.Tasks;

    /// <summary>
    /// Infrastructure layer service that contains common logic for most on demand services.
    /// </summary>
    /// <typeparam name="T">The model generic type parameter.</typeparam>
    public interface IBaseService<T>
    {
        /// <summary>
        /// Gets the value of the model.
        /// </summary>
        /// <returns>A <see cref="Task{T}"/> representing the result of the asynchronous operation.</returns>
        Task<T> GetAsync();
    }
}
