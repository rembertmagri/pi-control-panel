namespace PiControlPanel.Domain.Contracts.Infrastructure.Persistence
{
    using System.Threading.Tasks;

    public interface IBaseService<T>
    {
        Task<T> GetAsync();

        Task AddAsync(T model);

        Task UpdateAsync(T model);

        Task RemoveAsync(T model);
    }
}
