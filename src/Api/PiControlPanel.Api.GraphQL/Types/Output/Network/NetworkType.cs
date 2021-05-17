namespace PiControlPanel.Api.GraphQL.Types.Output.Network
{
    using System.Linq;
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Domain.Models.Hardware.Network;

    /// <summary>
    /// The Network GraphQL output type.
    /// </summary>
    public class NetworkType : ObjectGraphType<Network>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkType"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger instance.</param>
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
                    var name = context.Arguments["name"].Value;
                    return context.Source.NetworkInterfaces.SingleOrDefault(i => i.Name.Equals(name));
                });
        }
    }
}
