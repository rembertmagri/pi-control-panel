namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    /// <summary>
    /// The MemoryType GraphQL output type.
    /// </summary>
    /// <typeparam name="TMemory">The Memory generic type parameter.</typeparam>
    /// <typeparam name="TMemoryStatus">The MemoryStatus generic type parameter.</typeparam>
    public class MemoryType<TMemory, TMemoryStatus> : ObjectGraphType<TMemory>
        where TMemory : Memory
        where TMemoryStatus : MemoryStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryType{TMemory, TMemoryStatus}"/> class.
        /// </summary>
        /// <param name="memoryService">The application layer MemoryService.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public MemoryType(IMemoryService<TMemory, TMemoryStatus> memoryService, ILogger logger)
        {
            this.Field(x => x.Total);

            this.Field<MemoryStatusType<TMemoryStatus>>()
                .Name("Status")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Memory status field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await memoryService.GetLastStatusAsync();
                });

            this.Connection<MemoryStatusType<TMemoryStatus>>()
                .Name("Statuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("Memory statuses connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var statuses = await memoryService.GetStatusesAsync(pagingInput);

                    return statuses.ToConnection();
                });
        }
    }
}
