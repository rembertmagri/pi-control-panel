namespace PiControlPanel.Api.GraphQL.Types.Output.Network
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Network;

    public class NetworkType : ObjectGraphType<Network>
    {
        public NetworkType()
        {
            Field(x => x.Interfaces, false, typeof(ListGraphType<InterfaceType>)).Resolve(context => context.Source.Interfaces);
        }
    }
}
