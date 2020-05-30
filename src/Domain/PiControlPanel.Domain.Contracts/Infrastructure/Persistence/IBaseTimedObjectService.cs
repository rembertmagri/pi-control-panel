namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using PiControlPanel.Domain.Models.Hardware;
    using PiControlPanel.Domain.Models.Paging;

    /// <summary>
    /// Infrastructure layer service that contains common logic for persistence services of timed models.
    /// </summary>
    /// <typeparam name="T">The model generic type parameter.</typeparam>
    public interface IBaseTimedObjectService<T>
        where T : BaseTimedObject
    {
        /// <summary>
        /// Gets the most recent value that satisfies a specific condition.
        /// </summary>
        /// <param name="where">The condition to be considered.</param>
        /// <returns>A <see cref="Task{T}"/> representing the result of the asynchronous operation.</returns>
        Task<T> GetLastAsync(LambdaExpression where = null);

        /// <summary>
        /// Gets the all values that satisfy a specific condition.
        /// </summary>
        /// <param name="where">The condition to be considered.</param>
        /// <returns>A <see cref="Task{T}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<T>> GetAllAsync(LambdaExpression where = null);

        /// <summary>
        /// Gets the paged values that satisfy a specific condition.
        /// </summary>
        /// <param name="pagingInput">The paging information.</param>
        /// <param name="where">The condition to be considered.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<PagingOutput<T>> GetPageAsync(PagingInput pagingInput, LambdaExpression where = null);

        /// <summary>
        /// Persists the value in the database.
        /// </summary>
        /// <param name="model">The model to be created.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task AddAsync(T model);
    }
}
