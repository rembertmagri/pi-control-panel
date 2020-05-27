namespace PiControlPanel.Api.GraphQL.Types.Output
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    public class MemoryType<T, U> : ObjectGraphType<T>
        where T : Memory
        where U : MemoryStatus
    {
        public MemoryType(IMemoryService<T, U> memoryService, ILogger logger)
        {
            this.Field(x => x.Total);

            this.Field<MemoryStatusType<U>>()
                .Name("Status")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Memory status field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await memoryService.GetLastStatusAsync();
                });

            this.Connection<MemoryStatusType<U>>()
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
