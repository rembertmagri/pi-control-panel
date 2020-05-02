namespace PiControlPanel.Api.GraphQL.Types.Output.Network
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Network;

    public class InterfaceType : ObjectGraphType<Interface>
    {
        public InterfaceType()
        {
            Field(x => x.Name);
            Field(x => x.IpAddress);
            Field(x => x.SubnetMask);
            Field(x => x.DefaultGateway);
        }
    }
}
