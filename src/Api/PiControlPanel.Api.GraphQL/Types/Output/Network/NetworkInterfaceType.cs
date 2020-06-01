namespace PiControlPanel.Api.GraphQL.Types.Output.Network
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Network;

    /// <summary>
    /// The NetworkInterface GraphQL output type.
    /// </summary>
    public class NetworkInterfaceType : ObjectGraphType<NetworkInterface>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkInterfaceType"/> class.
        /// </summary>
        /// <param name="networkService">The application layer NetworkService.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public NetworkInterfaceType(INetworkService networkService, ILogger logger)
        {
            this.Field(x => x.Name);
            this.Field(x => x.IpAddress);
            this.Field(x => x.SubnetMask);
            this.Field(x => x.DefaultGateway);

            this.Field<NetworkInterfaceStatusType>()
                .Name("Status")
                .ResolveAsync(async context =>
                {
                    logger.Debug("Network interface status field");

                    return await networkService.GetLastNetworkInterfaceStatusAsync(context.Source.Name);
                });

            this.Connection<NetworkInterfaceStatusType>()
                .Name("Statuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("Network Interface statuses connection");

                    var pagingInput = context.GetPagingInput();
                    var networkInterfaceStatuses = await networkService.GetNetworkInterfaceStatusesAsync(context.Source.Name, pagingInput);

                    return networkInterfaceStatuses.ToConnection();
                });
        }
    }
}
