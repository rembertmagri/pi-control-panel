namespace PiControlPanel.Api.GraphQL.Types.Output.Disk
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    /// <summary>
    /// The FileSystemStatus GraphQL output type.
    /// </summary>
    public class FileSystemStatusType : ObjectGraphType<FileSystemStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemStatusType"/> class.
        /// </summary>
        public FileSystemStatusType()
        {
            this.Field(x => x.FileSystemName);
            this.Field(x => x.Used);
            this.Field(x => x.Available);
            this.Field<DateTimeGraphType>("dateTime");
        }
    }
}
