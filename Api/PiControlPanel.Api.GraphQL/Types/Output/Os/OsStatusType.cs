namespace PiControlPanel.Api.GraphQL.Types.Output.Os
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Os;

    public class OsStatusType : ObjectGraphType<OsStatus>
    {
        public OsStatusType()
        {
            Field(x => x.Uptime);
            Field<DateTimeGraphType>("dateTime");
        }
    }
}
