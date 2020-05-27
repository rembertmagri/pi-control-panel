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
            return this.mapper.Map<T>(entity);
        }

        public async Task AddAsync(T model)
        {
            var entity = this.mapper.Map<U>(model);
            this.repository.Create(entity);
            await this.unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(T model)
        {
            var entity = this.mapper.Map<U>(model);
            this.repository.Update(entity);
            await this.unitOfWork.CommitAsync();
        }

        public async Task RemoveAsync(T model)
        {
            var entity = this.mapper.Map<U>(model);
            this.repository.Remove(entity);
            await this.unitOfWork.CommitAsync();
        }

        protected virtual Task<U> GetFromRepository()
        {
            return this.repository.GetAsync();
        }
    }
}
