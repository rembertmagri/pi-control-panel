namespace PiControlPanel.Infrastructure.Persistence.Contracts.Repositories
{
    using PiControlPanel.Infrastructure.Persistence.Entities;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface defining base data repository functionality. Any entity-specific features have to be added to derived interface.
    /// </summary>
    ///
    public interface IRepositoryBase<T> where T : BaseEntity
    {
        /// <summary>
        /// Returns all records
        /// </summary>
        /// <returns>returns IQueryable</returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Returns multiple records for the given where expression
        /// </summary>
        /// <param name="where">expression to filter the records</param>
        /// <returns>returns IQueryable</returns>
        IQueryable<T> GetMany(Expression<Func<T, bool>> where);

        /// <summary>
        /// Returns true or false based on the where expression
        /// </summary>
        /// <param name="where">expression to filter the records</param>
        /// <returns>returns true or false</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> where);

        /// <summary>
        /// Returns single record. If not found then returns null
        /// </summary>
        /// <returns>returns object or null</returns>
        Task<T> GetAsync();

        /// <summary>
        /// Returns single record based on the where expression. If not found then returns null
        /// </summary>
        /// <param name="where">expression to filter the records</param>
        /// <returns>returns object or null</returns>
        Task<T> GetAsync(Expression<Func<T, bool>> where);

        /// <summary>
        /// Returns single record and related include expression. if not found then returns null
        /// </summary>
        /// <param name="include">expression to include the related tables data</param>
        /// <returns>returns object or null</returns>
        Task<T> GetAsync(Expression<Func<T, object>> include);

        /// <summary>
        /// Returns single record based on the where expression and related include expression. if not found then returns null
        /// </summary>
        /// <param name="where">expression to filter the records</param>
        /// <param name="include">expression to include the related tables data</param>
        /// <returns>returns object or null</returns>
        Task<T> GetAsync(Expression<Func<T, bool>> where, Expression<Func<T, object>> include);

        /// <summary>
        /// Updates the given entity in the database
        /// </summary>
        /// <param name="entity">entity to be updated in the database</param>
        void Update(T entity);

        /// <summary>
        /// Creates the given entity in the database
        /// </summary>
        /// <param name="entity">entity to be created in the database</param>
        void Create(T entity);

        /// <summary>
        /// Deletes the given entity in the database
        /// </summary>
        /// <param name="entity">entity to be deleted in the database</param>
        void Remove(T entity);
    }
}
