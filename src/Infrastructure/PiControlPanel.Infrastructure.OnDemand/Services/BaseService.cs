namespace PiControlPanel.Infrastructure.OnDemand.Services
{
    using System.Threading.Tasks;
    using NLog;
    using PiControlPanel.Domain.Contracts.Infrastructure.OnDemand;

    /// <inheritdoc/>
    public abstract class BaseService<T> : IBaseService<T>
    {
        /// <summary>
        /// The NLog logger instance.
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger instance.</param>
        public BaseService(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Task<T> GetAsync()
        {
            this.logger.Debug($"Infra layer -> BaseService<{typeof(T).Name}> -> GetAsync");
            var model = this.GetModel();
            return Task.FromResult(model);
        }

        /// <summary>
        /// Gets the model information on demand.
        /// </summary>
        /// <returns>The model information.</returns>
        protected abstract T GetModel();
    }
}
