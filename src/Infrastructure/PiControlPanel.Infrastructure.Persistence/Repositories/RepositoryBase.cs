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

    public class RepositoryBase<TObject> : IRepositoryBase<TObject> where TObject : BaseEntity
    {
        private readonly ControlPanelDbContext context;
        private readonly ILogger logger;

        private DbSet<TObject> dbSet => context.Set<TObject>();

        public IQueryable<TObject> Entities => dbSet;

        public RepositoryBase(ControlPanelDbContext dbContext, ILogger logger)
        {
            context = dbContext;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public IQueryable<TObject> GetAll()
        {
            logger.Debug("Infra layer -> RepositoryBase -> GetAll");
            return dbSet.AsQueryable();
        }

        /// <inheritdoc/>
        public IQueryable<TObject> GetMany(Expression<Func<TObject, bool>> where)
        {
            logger.Debug("Infra layer -> RepositoryBase -> GetMany");
            return dbSet.Where(where).AsQueryable();
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Expression<Func<TObject, bool>> where)
        {
            logger.Debug("Infra layer -> RepositoryBase -> ExistsAsync");
            return await dbSet.AnyAsync(where);

        }

        /// <inheritdoc/>
        public async Task<TObject> GetAsync()
        {
            logger.Debug("Infra layer -> RepositoryBase -> GetAsync");
            return await this.dbSet.FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<TObject> GetAsync(Expression<Func<TObject, bool>> where)
        {
            logger.Debug("Infra layer -> RepositoryBase -> GetAsync");
            return await this.dbSet.FirstOrDefaultAsync(where);
        }

        /// <inheritdoc/>
        public async Task<TObject> GetAsync(Expression<Func<TObject, object>> include)
        {
            logger.Debug("Infra layer -> RepositoryBase -> GetAsync");
            return await this.dbSet.Include(include).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<TObject> GetAsync(Expression<Func<TObject, bool>> where, Expression<Func<TObject, object>> include)
        {
            logger.Debug("Infra layer -> RepositoryBase -> GetAsync");
            return await this.dbSet.Include(include).FirstOrDefaultAsync(where);
        }

        /// <inheritdoc/>
        public void Update(TObject entity)
        {
            logger.Debug("Infra layer -> RepositoryBase -> Update");
            var entry = this.context.Entry(entity);
            this.dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }

        public async Task CreateManyAsync(TObject[] entities)
        {
            logger.Debug("Infra layer -> RepositoryBase -> CreateManyAsync");
            await this.dbSet.AddRangeAsync(entities);
        }

        public void Create(TObject entity)
        {
            logger.Debug("Infra layer -> RepositoryBase -> Create");
            this.dbSet.Add(entity);
        }

        public void RemoveMany(TObject[] entities)
        {
            logger.Debug("Infra layer -> RepositoryBase -> RemoveMany");
            this.dbSet.RemoveRange(entities);
        }

        public void Remove(TObject entity)
        {
            logger.Debug("Infra layer -> RepositoryBase -> Remove");
            this.dbSet.Remove(entity);
        }
    }
}
