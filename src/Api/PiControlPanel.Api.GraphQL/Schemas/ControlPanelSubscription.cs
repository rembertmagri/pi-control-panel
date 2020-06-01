namespace PiControlPanel.Api.GraphQL.Schemas
{
    using global::GraphQL;
    using global::GraphQL.Server.Transports.Subscriptions.Abstractions;
    using global::GraphQL.Types;
    using NLog;
    using PiControlPanel.Api.GraphQL.Extensions;
    using PiControlPanel.Api.GraphQL.Types.Output;
    using PiControlPanel.Api.GraphQL.Types.Output.Cpu;
    using PiControlPanel.Api.GraphQL.Types.Output.Disk;
    using PiControlPanel.Api.GraphQL.Types.Output.Network;
    using PiControlPanel.Api.GraphQL.Types.Output.Os;
    using PiControlPanel.Domain.Contracts.Application;
    using PiControlPanel.Domain.Models.Hardware.Memory;

    /// <summary>
    /// The root subscription GraphQL type.
    /// </summary>
    public class ControlPanelSubscription : ObjectGraphType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelSubscription"/> class.
        /// </summary>
        /// <param name="cpuService">The application layer CpuService.</param>
        /// <param name="diskService">The application layer DiskService.</param>
        /// <param name="randomAccessMemoryService">The application layer RandomAccessMemoryService.</param>
        /// <param name="swapMemoryService">The application layer SwapMemoryService.</param>
        /// <param name="operatingSystemService">The application layer OsService.</param>
        /// <param name="networkService">The application layer NetworkService.</param>
        /// <param name="logger">The NLog logger instance.</param>
        public ControlPanelSubscription(
            ICpuService cpuService,
            IDiskService diskService,
            IMemoryService<RandomAccessMemory, RandomAccessMemoryStatus> randomAccessMemoryService,
            IMemoryService<SwapMemory, SwapMemoryStatus> swapMemoryService,
            IOsService operatingSystemService,
            INetworkService networkService,
            ILogger logger)
        {
            this.FieldSubscribe<CpuLoadStatusType>(
                "CpuLoadStatus",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("CpuAverageLoad subscription");

                    var messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    var graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var userContext = graphQLUserContext.GetUserContext();

                    return cpuService.GetLoadStatusObservable();
                });

            this.FieldSubscribe<CpuTemperatureType>(
                "CpuTemperature",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("CpuTemperature subscription");

                    var messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    var graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var userContext = graphQLUserContext.GetUserContext();

                    return cpuService.GetTemperatureObservable();
                });

            this.FieldSubscribe<CpuFrequencyType>(
                "CpuFrequency",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("CpuFrequency subscription");

                    var messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    var graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var userContext = graphQLUserContext.GetUserContext();

                    return cpuService.GetFrequencyObservable();
                });

            this.FieldSubscribe<MemoryStatusType<RandomAccessMemoryStatus>>(
                "RamStatus",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("RamStatus subscription");

                    var messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    var graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var userContext = graphQLUserContext.GetUserContext();

                    return randomAccessMemoryService.GetStatusObservable();
                });

            this.FieldSubscribe<MemoryStatusType<SwapMemoryStatus>>(
                "SwapMemoryStatus",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("SwapMemoryStatus subscription");

                    var messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    var graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var userContext = graphQLUserContext.GetUserContext();

                    return swapMemoryService.GetStatusObservable();
                });

            this.FieldSubscribe<FileSystemStatusType>(
                "FileSystemStatus",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "FileSystemName" }),
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("FileSystemStatus subscription");

                    var messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    var graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var userContext = graphQLUserContext.GetUserContext();

                    var fileSystemName = context.GetArgument<string>("fileSystemName");

                    return diskService.GetFileSystemStatusObservable(fileSystemName);
                });

            this.FieldSubscribe<OsStatusType>(
                "OsStatus",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("OsStatus subscription");

                    var messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    var graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var userContext = graphQLUserContext.GetUserContext();

                    return operatingSystemService.GetStatusObservable();
                });

            this.FieldSubscribe<NetworkInterfaceStatusType>(
                "NetworkInterfaceStatus",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "NetworkInterfaceName" }),
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("NetworkInterfaceStatus subscription");

                    var messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    var graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var userContext = graphQLUserContext.GetUserContext();

                    var networkInterfaceName = context.GetArgument<string>("networkInterfaceName");

                    return networkService.GetNetworkInterfaceStatusObservable(networkInterfaceName);
                });
        }
    }
}
