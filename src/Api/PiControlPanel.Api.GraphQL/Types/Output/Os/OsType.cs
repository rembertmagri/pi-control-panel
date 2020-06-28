namespace PiControlPanel.Api.GraphQL.Types.Output.Os
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Os;

    /// <summary>
    /// The Os GraphQL output type.
    /// </summary>
    public class OsType : ObjectGraphType<Os>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OsType"/> class.
        /// </summary>
        /// <param name="operatingSystemService">The application layer OsService.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public OsType(IOsService operatingSystemService, ILogger logger)
        {
            this.Field(x => x.Name);
            this.Field(x => x.Kernel);
            this.Field(x => x.Hostname);
            this.Field(x => x.SshStarted);
            this.Field(x => x.SshPort);
            this.Field(x => x.UpgradeablePackages);

            this.Field<OsStatusType>()
                .Name("Status")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Os status field");

                    return await operatingSystemService.GetLastStatusAsync();
                });

            this.Connection<OsStatusType>()
                .Name("Statuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("Os statuses connection");

                    var pagingInput = context.GetPagingInput();
                    var statuses = await operatingSystemService.GetStatusesAsync(pagingInput);

                    return statuses.ToConnection();
                });
        }
    }
}
