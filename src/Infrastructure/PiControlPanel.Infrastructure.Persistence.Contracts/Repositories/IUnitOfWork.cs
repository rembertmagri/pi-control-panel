namespace PiControlPanel.Infrastructure.Persistence.Contracts.Repositories
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface defining the unit of work used to retrieve and store data.
    /// </summary>
    public interface IUnitOfWork : IDisposable, IRepositoryContainer
    {
        /// <summary>
        /// Commits all changes.
        /// </summary>
        void Commit();

        /// <summary>
        /// Commits all changes asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CommitAsync();

        /// <summary>
        /// Discards all changes that has not been committed.
        /// </summary>
        void RejectChanges();
    }
}
