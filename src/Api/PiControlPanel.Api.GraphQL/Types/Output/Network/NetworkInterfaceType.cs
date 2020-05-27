namespace PiControlPanel.Api.GraphQL.Types.Output.Network
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Network;

    public class NetworkInterfaceType : ObjectGraphType<NetworkInterface>
    {
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
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await networkService.GetLastNetworkInterfaceStatusAsync(context.Source.Name);
                });

            this.Connection<NetworkInterfaceStatusType>()
                .Name("Statuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Debug("Network Interface statuses connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var networkInterfaceStatuses = await networkService.GetNetworkInterfaceStatusesAsync(context.Source.Name, pagingInput);

                    return networkInterfaceStatuses.ToConnection();
                });
        }
    }
}
