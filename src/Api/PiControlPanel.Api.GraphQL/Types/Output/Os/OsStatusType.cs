namespace PiControlPanel.Api.GraphQL.Types.Output.Os
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Os;

    public class OsStatusType : ObjectGraphType<OsStatus>
    {
        public OsStatusType()
        {
            this.Field(x => x.Uptime);
            this.Field<DateTimeGraphType>("dateTime");
        }
    }
}
