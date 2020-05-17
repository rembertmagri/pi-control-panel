namespace PiControlPanel.Domain.Contracts.Application
{
    using System.Threading.Tasks;

    public interface IBaseService<T>
    {
        Task<T> GetAsync();

        Task SaveAsync();
    }
}
