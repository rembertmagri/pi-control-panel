namespace PiControlPanel.Domain.Contracts.Infrastructure.OnDemand
{
    using System.Threading.Tasks;

    public interface IBaseService<T>
    {
        Task<T> GetAsync();
    }
}
