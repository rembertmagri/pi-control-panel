﻿namespace PiControlPanel.Infrastructure.Persistence.Services
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
    public abstract class BaseTimedService<T, U> : IBaseTimedObjectService<T>
        where T : BaseTimedObject
        where U : BaseTimedEntity
    {
        protected IRepositoryBase<U> repository;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;
        protected readonly ILogger logger;

        public BaseTimedService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<T> GetLastAsync(LambdaExpression where = null)
        {
            var entity = await this.GetAll(where)
                .OrderByDescending(t => t.DateTime).FirstOrDefaultAsync();
            return this.mapper.Map<T>(entity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> GetAllAsync(LambdaExpression where = null)
        {
            var entities = await this.GetAll(where)
                .OrderBy(t => t.DateTime).ToListAsync();
            return this.mapper.Map<List<T>>(entities);
        }

        /// <inheritdoc/>
        public async Task<PagingOutput<T>> GetPageAsync(PagingInput pagingInput, LambdaExpression where = null)
        {
            IQueryable<U> entities = this.GetAll(where);

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
            return new PagingOutput<T>()
            {
                TotalCount = totalCount,
                Result = this.mapper.Map<List<T>>(result),
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage
            };
        }

        /// <inheritdoc/>
        public async Task AddAsync(T model)
        {
            var entity = this.mapper.Map<U>(model);
            this.repository.Create(entity);
            await this.unitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        protected virtual IQueryable<U> GetAll(LambdaExpression where = null)
        {
            if (where == null)
            {
                return this.repository.GetAll();
            }

            return this.repository.GetMany(where as Expression<Func<U, bool>>);
        }
    }
}
