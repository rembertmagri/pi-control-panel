namespace PiControlPanel.Api.GraphQL.Types.Output.Network
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Network;

    public class NetworkInterfaceStatusType : ObjectGraphType<NetworkInterfaceStatus>
    {
        public NetworkInterfaceStatusType()
        {
            Field(x => x.NetworkInterfaceName);
            Field(x => x.TotalReceived);
            Field(x => x.TotalSent);
            Field(x => x.ReceiveSpeed);
            Field(x => x.SendSpeed);
            Field<DateTimeGraphType>("dateTime");
        }
    }
}
