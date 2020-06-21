namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;

    /// <inheritdoc/>
    public abstract class BaseService<T> : IBaseService<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger instance.</param>
        public BaseService(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the NLog logger instance.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc/>
        public Task<T> GetAsync()
        {
            this.Logger.Debug($"Infra layer -> BaseService<{typeof(T).Name}> -> GetAsync");
            return this.GetModelAsync();
        }

        /// <summary>
        /// Gets the model information on demand.
        /// </summary>
        /// <returns>The model information.</returns>
        protected abstract Task<T> GetModelAsync();
    }
}
