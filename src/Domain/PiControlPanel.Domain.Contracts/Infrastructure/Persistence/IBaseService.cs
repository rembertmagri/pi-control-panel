namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence
{
    using System.Threading.Tasks;

    /// <summary>
    /// Infrastructure layer service that contains common logic for persistence services.
    /// </summary>
    /// <typeparam name="T">The model generic type parameter.</typeparam>
    public interface IBaseService<T>
    {
        /// <summary>
        /// Gets the value from the database.
        /// </summary>
        /// <returns>A <see cref="Task{T}"/> representing the result of the asynchronous operation.</returns>
        Task<T> GetAsync();

        /// <summary>
        /// Persists the value in the database.
        /// </summary>
        /// <param name="model">The model to be created.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task AddAsync(T model);

        /// <summary>
        /// Updates the value in the database.
        /// </summary>
        /// <param name="model">The model to be updated.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateAsync(T model);

        /// <summary>
        /// Removes the value from the database.
        /// </summary>
        /// <param name="model">The model to be deleted.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task RemoveAsync(T model);
    }
}
