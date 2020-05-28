namespace PiControlPanel.Api.GraphQL.Types.Output.Disk
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    /// <summary>
    /// The FileSystem GraphQL output type.
    /// </summary>
    public class FileSystemType : ObjectGraphType<FileSystem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemType"/> class.
        /// </summary>
        /// <param name="fileSystemService">The application layer DiskService.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public FileSystemType(IDiskService fileSystemService, ILogger logger)
        {
            this.Field(x => x.Name);
            this.Field(x => x.Type);
            this.Field(x => x.Total);

            this.Field<FileSystemStatusType>()
                .Name("Status")
                .ResolveAsync(async context =>
                {
                    logger.Debug("File System status field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await fileSystemService.GetLastFileSystemStatusAsync(context.Source.Name);
                });

            this.Connection<FileSystemStatusType>()
                .Name("Statuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("File System statuses connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var statuses = await fileSystemService.GetFileSystemStatusesAsync(context.Source.Name, pagingInput);

                    return statuses.ToConnection();
                });
        }
    }
}
