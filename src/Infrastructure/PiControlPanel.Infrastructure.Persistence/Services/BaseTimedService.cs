namespace PiControlPanel.Infrastructure.Persistence.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence;
    using PiControlPanel.Domain.Models.Hardware;
    using PiControlPanel.Domain.Models.Paging;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    using PiControlPanel.Infrastructure.Persistence.Entities;

    /// <inheritdoc/>
    public abstract class BaseTimedService<TModel, TEntity> : IBaseTimedObjectService<TModel>
        where TModel : BaseTimedObject
        where TEntity : BaseTimedEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTimedService{TModel, TEntity}"/> class.
        /// </summary>
        /// <param name="repository">The repository for the entity.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public BaseTimedService(IRepositoryBase<TEntity> repository, IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
        {
            this.Repository = repository;
            this.UnitOfWork = unitOfWork;
            this.Mapper = mapper;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the repository for the entity.
        /// </summary>
        protected IRepositoryBase<TEntity> Repository { get; }

        /// <summary>
        /// Gets the unit of work.
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Gets the mapper configuration.
        /// </summary>
        protected IMapper Mapper { get; }

        /// <summary>
        /// Gets the NLog logger instance.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc/>
        public async Task<TModel> GetLastAsync(LambdaExpression where = null)
        {
            var entity = await this.GetAll(where)
                .OrderByDescending(t => t.DateTime).FirstOrDefaultAsync();
            return this.Mapper.Map<TModel>(entity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TModel>> GetAllAsync(LambdaExpression where = null)
        {
            var entities = await this.GetAll(where)
                .OrderBy(t => t.DateTime).ToListAsync();
            return this.Mapper.Map<List<TModel>>(entities);
        }

        /// <inheritdoc/>
        public async Task<PagingOutput<TModel>> GetPageAsync(PagingInput pagingInput, LambdaExpression where = null)
        {
            IQueryable<TEntity> entities = this.GetAll(where);

            var totalCount = entities.Count();
            var totalSkipped = 0;
            var hasNextPage = false;
            var hasPreviousPage = false;

            if (pagingInput.First.HasValue)
            {
                entities = entities.OrderBy(t => t.DateTime);
                if (!string.IsNullOrEmpty(pagingInput.After))
                {
                    var afterEntity = await entities
                        .FirstAsync(e => e.ID == Guid.Parse(pagingInput.After));
                    if (afterEntity == null)
                    {
                        throw new ArgumentOutOfRangeException("After", $"No entity found with id={pagingInput.After}");
                    }

                    totalSkipped = entities
                        .Count(e => e.DateTime <= afterEntity.DateTime);
                    entities = entities
                        .Where(e => e.DateTime > afterEntity.DateTime);
                }

                entities = entities
                        .Take(pagingInput.First.Value);
                hasNextPage = totalSkipped + pagingInput.First.Value < totalCount;
                hasPreviousPage = totalSkipped != 0;
            }
            else if (pagingInput.Last.HasValue)
            {
                entities = entities.OrderByDescending(t => t.DateTime);
                if (!string.IsNullOrEmpty(pagingInput.Before))
                {
                    var beforeEntity = await entities
                        .FirstAsync(e => e.ID == Guid.Parse(pagingInput.Before));
                    if (beforeEntity == null)
                    {
                        throw new ArgumentOutOfRangeException("Before", $"No entity found with id={pagingInput.Before}");
                    }

                    totalSkipped = entities
                        .Count(e => e.DateTime >= beforeEntity.DateTime);
                    entities = entities
                        .Where(e => e.DateTime < beforeEntity.DateTime);
                }

                entities = entities
                        .Take(pagingInput.Last.Value)
                        .OrderBy(t => t.DateTime);
                hasNextPage = totalSkipped != 0;
                hasPreviousPage = totalSkipped + pagingInput.Last.Value < totalCount;
            }

            var result = await entities.ToListAsync();
            return new PagingOutput<TModel>()
            {
                TotalCount = totalCount,
                Result = this.Mapper.Map<List<TModel>>(result),
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage
            };
        }

        /// <inheritdoc/>
        public async Task AddAsync(TModel model)
        {
            var entity = this.Mapper.Map<TEntity>(model);
            this.Repository.Create(entity);
            await this.UnitOfWork.CommitAsync();
        }

        /// <summary>
        /// Retrieves all entities that match the condition from the repository.
        /// </summary>
        /// <param name="where">The condition to be used to filter the results.</param>
        /// <returns>An IQueryable of the entities.</returns>
        protected virtual IQueryable<TEntity> GetAll(LambdaExpression where = null)
        {
            if (where == null)
            {
                return this.Repository.GetAll();
            }

            return this.Repository.GetMany(where as Expression<Func<TEntity, bool>>);
        }
    }
}
