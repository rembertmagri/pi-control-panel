namespace PiControlPanel.Api.GraphQL.Types.Output.Disk
{
    using System.Linq;
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Domain.Models.Hardware.Disk;

    /// <summary>
    /// The Disk GraphQL output type.
    /// </summary>
    public class DiskType : ObjectGraphType<Disk>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiskType"/> class.
        /// </summary>
        /// <param name="logger">The NLog logger instance.</param>
        public DiskType(ILogger logger)
        {
            this.Field(x => x.FileSystems, false, typeof(ListGraphType<FileSystemType>))
                .Resolve(context => context.Source.FileSystems);

            this.Field<FileSystemType>(
                "FileSystem",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "Name" }),
                resolve: context =>
                {
                    logger.Debug("FileSystem field");
                    var name = context.Arguments["name"].Value;
                    return context.Source.FileSystems.SingleOrDefault(i => i.Name.Equals(name));
                });
        }
    }
}
