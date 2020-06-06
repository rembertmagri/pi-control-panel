namespace PiControlPanel.Infrastructure.Persistence.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    using PiControlPanel.Infrastructure.Persistence.Entities;

    /// <inheritdoc/>
    public abstract class BaseService<TModel, TEntity> : IBaseService<TModel>
        where TEntity : BaseEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{TModel, TEntity}"/> class.
        /// </summary>
        /// <param name="repository">The repository for the entity.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public BaseService(
            IRepositoryBase<TEntity> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger logger)
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
        public async Task<TModel> GetAsync()
        {
            var entity = await this.GetFromRepository();
            return this.Mapper.Map<TModel>(entity);
        }

        /// <inheritdoc/>
        public async Task AddAsync(TModel model)
        {
            var entity = this.Mapper.Map<TEntity>(model);
            this.Repository.Create(entity);
            await this.UnitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        public virtual async Task UpdateAsync(TModel model)
        {
            var entity = this.Mapper.Map<TEntity>(model);
            this.Repository.Update(entity);
            await this.UnitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(TModel model)
        {
            var entity = this.Mapper.Map<TEntity>(model);
            this.Repository.Remove(entity);
            await this.UnitOfWork.CommitAsync();
        }

        /// <summary>
        /// Retrieves the entity from the repository.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task<TEntity> GetFromRepository()
        {
            return this.Repository.GetAsync();
        }
    }
}
