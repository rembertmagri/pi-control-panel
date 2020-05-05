﻿namespace PiControlPanel.Api.GraphQL.Schemas
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

    public class ControlPanelSubscription : ObjectGraphType
    {
        public ControlPanelSubscription(ICpuService cpuService, IDiskService diskService,
            IMemoryService<RandomAccessMemory, RandomAccessMemoryStatus> randomAccessMemoryService,
            IMemoryService<SwapMemory, SwapMemoryStatus> swapMemoryService, IOsService osService,
            INetworkService networkService, ILogger logger)
        {
            FieldSubscribe<CpuLoadStatusType>(
                "CpuLoadStatus",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("CpuAverageLoad subscription");
                    MessageHandlingContext messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    GraphQLUserContext graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return cpuService.GetLoadStatusObservable();
                });

            FieldSubscribe<CpuTemperatureType>(
                "CpuTemperature",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("CpuTemperature subscription");
                    MessageHandlingContext messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    GraphQLUserContext graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return cpuService.GetTemperatureObservable();
                });

            FieldSubscribe<CpuFrequencyType>(
                "CpuFrequency",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("CpuFrequency subscription");
                    MessageHandlingContext messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    GraphQLUserContext graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return cpuService.GetFrequencyObservable();
                });

            FieldSubscribe<MemoryStatusType<RandomAccessMemoryStatus>>(
                "RamStatus",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("RamStatus subscription");
                    MessageHandlingContext messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    GraphQLUserContext graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return randomAccessMemoryService.GetStatusObservable();
                });

            FieldSubscribe<MemoryStatusType<SwapMemoryStatus>>(
                "SwapMemoryStatus",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("SwapMemoryStatus subscription");
                    MessageHandlingContext messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    GraphQLUserContext graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return swapMemoryService.GetStatusObservable();
                });

            FieldSubscribe<FileSystemStatusType>(
                "FileSystemStatus",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "FileSystemName" }
                ),
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("FileSystemStatus subscription");
                    MessageHandlingContext messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    GraphQLUserContext graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var fileSystemName = context.GetArgument<string>("fileSystemName");

                    return diskService.GetFileSystemStatusObservable(fileSystemName);
                });

            FieldSubscribe<OsStatusType>(
                "OsStatus",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("OsStatus subscription");
                    MessageHandlingContext messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    GraphQLUserContext graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return osService.GetStatusObservable();
                });

            FieldSubscribe<NetworkInterfaceStatusType>(
                "NetworkInterfaceStatus",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "NetworkInterfaceName" }
                ),
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("NetworkInterfaceStatus subscription");
                    MessageHandlingContext messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    GraphQLUserContext graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    var networkInterfaceName = context.GetArgument<string>("networkInterfaceName");

                    return networkService.GetNetworkInterfaceStatusObservable(networkInterfaceName);
                });
        }
    }
}
