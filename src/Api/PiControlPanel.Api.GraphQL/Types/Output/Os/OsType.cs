namespace PiControlPanel.Api.GraphQL.Types.Output.Os
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Os;

    public class OsType : ObjectGraphType<Os>
    {
        public OsType(IOsService operatingSystemService, ILogger logger)
        {
            this.Field(x => x.Name);
            this.Field(x => x.Kernel);
            this.Field(x => x.Hostname);

            this.Field<OsStatusType>()
                .Name("Status")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Os status field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await operatingSystemService.GetLastStatusAsync();
                });

            this.Connection<OsStatusType>()
                .Name("Statuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("Os statuses connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var statuses = await operatingSystemService.GetStatusesAsync(pagingInput);

                    return statuses.ToConnection();
                });
        }
    }
}
