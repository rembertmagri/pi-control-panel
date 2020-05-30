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
        /// The repository for the entity.
        /// </summary>
        protected IRepositoryBase<TEntity> repository;

        /// <summary>
        /// The unit of work.
        /// </summary>
        protected readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The mapper configuration.
        /// </summary>
        protected readonly IMapper mapper;

        /// <summary>
        /// The NLog logger instance.
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{TModel, TEntity}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper configuration.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public BaseService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<TModel> GetAsync()
        {
            var entity = await this.GetFromRepository();
            return this.mapper.Map<TModel>(entity);
        }

        /// <inheritdoc/>
        public async Task AddAsync(TModel model)
        {
            var entity = this.mapper.Map<TEntity>(model);
            this.repository.Create(entity);
            await this.unitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(TModel model)
        {
            var entity = this.mapper.Map<TEntity>(model);
            this.repository.Update(entity);
            await this.unitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(TModel model)
        {
            var entity = this.mapper.Map<TEntity>(model);
            this.repository.Remove(entity);
            await this.unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Retrieves the entity from the repository.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task<TEntity> GetFromRepository()
        {
            return this.repository.GetAsync();
        }
    }
}
