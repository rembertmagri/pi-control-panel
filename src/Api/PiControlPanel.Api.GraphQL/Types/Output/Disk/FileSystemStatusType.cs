namespace PiControlPanel.Api.GraphQL.Types.Output.Disk
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    public class FileSystemStatusType : ObjectGraphType<FileSystemStatus>
    {
        public FileSystemStatusType()
        {
            this.Field(x => x.FileSystemName);
            this.Field(x => x.Used);
            this.Field(x => x.Available);
            this.Field<DateTimeGraphType>("dateTime");
        }
    }
}
