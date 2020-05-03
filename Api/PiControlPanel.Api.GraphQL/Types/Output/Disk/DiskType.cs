namespace PiControlPanel.Api.GraphQL.Types.Output.Disk
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    public class DiskType : ObjectGraphType<Disk>
    {
        public DiskType(IDiskService diskService, ILogger logger)
        {
            Field(x => x.FileSystem);
            Field(x => x.Type);
            Field(x => x.Total);

            Field<DiskStatusType>()
                .Name("Status")
                .ResolveAsync(async context =>
                {
                    logger.Trace("Disk status field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await diskService.GetLastStatusAsync();
                });

            Connection<DiskStatusType>()
                .Name("Statuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Trace("Disk statuses connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var statuses = await diskService.GetStatusesAsync(pagingInput);

                    return statuses.ToConnection();
                });
        }
    }
}
