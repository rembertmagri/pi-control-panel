namespace PiControlPanel.Api.GraphQL.Types.Output.Network
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Network;

    public class NetworkInterfaceStatusType : ObjectGraphType<NetworkInterfaceStatus>
    {
        public NetworkInterfaceStatusType()
        {
            this.Field(x => x.NetworkInterfaceName);
            this.Field(x => x.TotalReceived);
            this.Field(x => x.TotalSent);
            this.Field(x => x.ReceiveSpeed);
            this.Field(x => x.SendSpeed);
            this.Field<DateTimeGraphType>("dateTime");
        }
    }
}
