namespace PiControlPanel.Api.GraphQL.Types.Output.Disk
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    public class FileSystemType : ObjectGraphType<FileSystem>
    {
        public FileSystemType(IDiskService fileSystemService, ILogger logger)
        {
            Field(x => x.Name);
            Field(x => x.Type);
            Field(x => x.Total);

            Field<FileSystemStatusType>()
                .Name("Status")
                .ResolveAsync(async context =>
                {
                    logger.Trace("File System status field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await fileSystemService.GetLastFileSystemStatusAsync(context.Source.Name);
                });

            Connection<FileSystemStatusType>()
                .Name("Statuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Trace("File System statuses connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var statuses = await fileSystemService.GetFileSystemStatusesAsync(context.Source.Name, pagingInput);

                    return statuses.ToConnection();
                });
        }
    }
}
