namespace PiControlPanel.Api.GraphQL.Types.Output.Os
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Os;

    public class OsType : ObjectGraphType<Os>
    {
        public OsType(IOsService osService, ILogger logger)
        {
            Field(x => x.Name);
            Field(x => x.Kernel);
            Field(x => x.Hostname);

            Field<OsStatusType>()
                .Name("Status")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Os status field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await osService.GetLastStatusAsync();
                });

            Connection<OsStatusType>()
                .Name("Statuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("Os statuses connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var statuses = await osService.GetStatusesAsync(pagingInput);

                    return statuses.ToConnection();
                });
        }
    }
}
