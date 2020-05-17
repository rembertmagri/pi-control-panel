namespace PiControlPanel.Api.GraphQL.Types.Output.Network
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using System.Linq;

    public class NetworkType : ObjectGraphType<Network>
    {
        public NetworkType(ILogger logger)
        {
            Field(x => x.NetworkInterfaces, false, typeof(ListGraphType<NetworkInterfaceType>))
                .Resolve(context => context.Source.NetworkInterfaces);

            Field<NetworkInterfaceType>(
                "NetworkInterface",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "Name" }
                ),
                resolve: context =>
                {
                    logger.Debug("NetworkInterface field");
                    var name = context.GetArgument<string>("name");
                    return context.Source.NetworkInterfaces.SingleOrDefault(i => i.Name == name);
                });
        }
    }
}
