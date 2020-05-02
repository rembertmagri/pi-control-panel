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
            Field(x => x.Name);
            Field(x => x.IpAddress);
            Field(x => x.SubnetMask);
            Field(x => x.DefaultGateway);

            Field<NetworkInterfaceStatusType>()
                .Name("Status")
                .ResolveAsync(async context =>
                {
                    logger.Info("Network interface status field");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return await networkService.GetLastNetworkInterfaceStatusAsync(context.Source.Name);
                });

            Connection<NetworkInterfaceStatusType>()
                .Name("Statuses")
                .Bidirectional()
                .ResolveAsync(async context =>
                {
                    logger.Info("Network Interface statuses connection");
                    GraphQLUserContext graphQLUserContext = context.UserContext as GraphQLUserContext;
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var pagingInput = context.GetPagingInput();
                    var networkInterfaceStatuses = await networkService.GetNetworkInterfaceStatusesAsync(context.Source.Name, pagingInput);

                    return networkInterfaceStatuses.ToConnection();
                });
        }
    }
}
