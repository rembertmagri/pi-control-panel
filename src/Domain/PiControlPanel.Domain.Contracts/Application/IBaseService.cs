namespace PiControlPanel.Domain.Contracts.Application
{
    using System.Threading.Tasks;

    /// <summary>
    /// Application layer service that contains common logic for most services.
    /// </summary>
    /// <typeparam name="T">The model generic type parameter.</typeparam>
    public interface IBaseService<T>
    {

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <returns>A <see cref="Task{T}"/> representing the result of the asynchronous operation.</returns>
        Task<T> GetAsync();

        /// <summary>
        /// Retrieves and saves the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task SaveAsync();
    }
}
