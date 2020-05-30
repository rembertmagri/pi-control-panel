namespace PiControlPanel.Infrastructure.Persistence.Repositories
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NLog;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    using PiControlPanel.Infrastructure.Persistence.Entities;

    /// <inheritdoc/>
    public class RepositoryBase<TObject> : IRepositoryBase<TObject>
        where TObject : BaseEntity
    {
        private readonly ControlPanelDbContext context;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TObject}"/> class.
        /// </summary>
        /// <param name="databaseContext">The database context.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public RepositoryBase(ControlPanelDbContext databaseContext, ILogger logger)
        {
            this.context = databaseContext;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the dbSet based on the database context.
        /// </summary>
        public IQueryable<TObject> Entities => this.DbSet;

        private DbSet<TObject> DbSet => this.context.Set<TObject>();

        /// <inheritdoc/>
        public IQueryable<TObject> GetAll()
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> GetAll");
            return this.DbSet.AsQueryable();
        }

        /// <inheritdoc/>
        public IQueryable<TObject> GetMany(Expression<Func<TObject, bool>> where)
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> GetMany");
            return this.DbSet.Where(where).AsQueryable();
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Expression<Func<TObject, bool>> where)
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> ExistsAsync");
            return await this.DbSet.AnyAsync(where);

        }

        /// <inheritdoc/>
        public async Task<TObject> GetAsync()
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> GetAsync");
            return await this.DbSet.FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<TObject> GetAsync(Expression<Func<TObject, bool>> where)
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> GetAsync");
            return await this.DbSet.FirstOrDefaultAsync(where);
        }

        /// <inheritdoc/>
        public async Task<TObject> GetAsync(Expression<Func<TObject, object>> include)
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> GetAsync");
            return await this.DbSet.Include(include).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<TObject> GetAsync(Expression<Func<TObject, bool>> where, Expression<Func<TObject, object>> include)
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> GetAsync");
            return await this.DbSet.Include(include).FirstOrDefaultAsync(where);
        }

        /// <inheritdoc/>
        public void Update(TObject entity)
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> Update");
            var entry = this.context.Entry(entity);
            this.DbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }

        /// <inheritdoc/>
        public async Task CreateManyAsync(TObject[] entities)
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> CreateManyAsync");
            await this.DbSet.AddRangeAsync(entities);
        }

        /// <inheritdoc/>
        public void Create(TObject entity)
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> Create");
            this.DbSet.Add(entity);
        }

        /// <inheritdoc/>
        public void RemoveMany(TObject[] entities)
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> RemoveMany");
            this.DbSet.RemoveRange(entities);
        }

        /// <inheritdoc/>
        public void Remove(TObject entity)
        {
            this.logger.Debug("Infra layer -> RepositoryBase -> Remove");
            this.DbSet.Remove(entity);
        }
    }
}
