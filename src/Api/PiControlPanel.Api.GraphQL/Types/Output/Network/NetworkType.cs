namespace PiControlPanel.Api.GraphQL.Types.Output.Network
{
    using System.Linq;
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Domain.Models.Hardware.Network;

    public class NetworkType : ObjectGraphType<Network>
    {
        public NetworkType(ILogger logger)
        {
            this.Field(x => x.NetworkInterfaces, false, typeof(ListGraphType<NetworkInterfaceType>))
                .Resolve(context => context.Source.NetworkInterfaces);

            this.Field<NetworkInterfaceType>(
                "NetworkInterface",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "Name" }),
                resolve: context =>
                {
                    logger.Debug("NetworkInterface field");
                    var name = context.GetArgument<string>("name");
                    return context.Source.NetworkInterfaces.SingleOrDefault(i => i.Name == name);
                });
        }
    }
}
