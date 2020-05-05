namespace PiControlPanel.Api.GraphQL.Types.Output.Disk
{
    using global::GraphQL.Types;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    public class FileSystemStatusType : ObjectGraphType<FileSystemStatus>
    {
        public FileSystemStatusType()
        {
            Field(x => x.Used);
            Field(x => x.Available);
            Field<DateTimeGraphType>("dateTime");
        }
    }
}
