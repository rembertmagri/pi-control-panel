namespace PiControlPanel.Api.GraphQL.Types.Output.Os
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Os;

    /// <summary>
    /// The OsStatus GraphQL output type.
    /// </summary>
    public class OsStatusType : ObjectGraphType<OsStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OsStatusType"/> class.
        /// </summary>
        public OsStatusType()
        {
            this.Field(x => x.Uptime);
            this.Field(x => x.UpgradeablePackages);
            this.Field<DateTimeGraphType>("dateTime");
        }
    }
}
