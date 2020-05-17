namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence
{
    using PiControlPanel.Domain.Models.Hardware;
    using PiControlPanel.Domain.Models.Paging;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IBaseTimedObjectService<T> where T : BaseTimedObject
    {
        Task<T> GetLastAsync(LambdaExpression where = null);

        Task<IEnumerable<T>> GetAllAsync(LambdaExpression where = null);

        Task<PagingOutput<T>> GetPageAsync(PagingInput pagingInput, LambdaExpression where = null);

        Task AddAsync(T model);
    }
}
