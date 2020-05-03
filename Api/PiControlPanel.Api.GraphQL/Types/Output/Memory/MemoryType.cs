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
            Field(x => x.Total);

            Field<MemoryStatusType<U>>()
                .Name("Status")
                .ResolveAsync(async context =>
                {
                    logger.Trace("Memory status field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await memoryService.GetLastStatusAsync();
                });

            Connection<MemoryStatusType<U>>()
                .Name("Statuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Trace("Memory statuses connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var statuses = await memoryService.GetStatusesAsync(pagingInput);

                    return statuses.ToConnection();
                });
        }
    }
}
