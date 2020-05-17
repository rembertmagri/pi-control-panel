namespace PiControlPanel.Api.GraphQL.Types.Output.Disk
{
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using System.Linq;

    public class DiskType : ObjectGraphType<Disk>
    {
        public DiskType(ILogger logger)
        {
            Field(x => x.FileSystems, false, typeof(ListGraphType<FileSystemType>))
                .Resolve(context => context.Source.FileSystems);

            Field<FileSystemType>(
                "FileSystem",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "Name" }
                ),
                resolve: context =>
                {
                    logger.Debug("FileSystem field");
                    var name = context.GetArgument<string>("name");
                    return context.Source.FileSystems.SingleOrDefault(i => i.Name == name);
                });
        }
    }
}
