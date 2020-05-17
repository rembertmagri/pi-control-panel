namespace PiControlPanel.Infrastructure.Persistence.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.Persistence;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    using PiControlPanel.Infrastructure.Persistence.Entities;

    public abstract class BaseService<T, U> : IBaseService<T> where U : BaseEntity
    {
        protected IRepositoryBase<U> repository;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;
        protected readonly ILogger logger;

        public BaseService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<T> GetAsync()
        {
            var entity = await this.GetFromRepository();
            return mapper.Map<T>(entity);
        }
        
        public async Task AddAsync(T model)
        {
            var entity = mapper.Map<U>(model);
            repository.Create(entity);
            await this.unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(T model)
        {
            var entity = mapper.Map<U>(model);
            repository.Update(entity);
            await this.unitOfWork.CommitAsync();
        }

        public async Task RemoveAsync(T model)
        {
            var entity = mapper.Map<U>(model);
            repository.Remove(entity);
            await this.unitOfWork.CommitAsync();
        }

        protected virtual Task<U> GetFromRepository()
        {
            return repository.GetAsync();
        }
    }
}
