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
    using PiControlPanel.Domain.Contracts.Application;

    public class ControlPanelSubscription : ObjectGraphType
    {
        public ControlPanelSubscription(ICpuService cpuService, IMemoryService memoryService,
            IDiskService diskService, ILogger logger)
        {
            FieldSubscribe<CpuAverageLoadType>(
                "CpuAverageLoad",
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

                    return cpuService.GetAverageLoadObservable();
                });

            FieldSubscribe<CpuRealTimeLoadType>(
                "CpuRealTimeLoad",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("CpuRealTimeLoad subscription");
                    MessageHandlingContext messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    GraphQLUserContext graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return cpuService.GetRealTimeLoadObservable();
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

            FieldSubscribe<MemoryStatusType>(
                "MemoryStatus",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("MemoryStatus subscription");
                    MessageHandlingContext messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    GraphQLUserContext graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return memoryService.GetStatusObservable();
                });

            FieldSubscribe<DiskStatusType>(
                "DiskStatus",
                resolve: context =>
                {
                    return context.Source;
                },
                subscribe: context =>
                {
                    logger.Info("DiskStatus subscription");
                    MessageHandlingContext messageHandlingContext = context.UserContext.As<MessageHandlingContext>();
                    GraphQLUserContext graphQLUserContext = messageHandlingContext.Get<GraphQLUserContext>("GraphQLUserContext");
                    var businessContext = graphQLUserContext.GetBusinessContext();

                    return diskService.GetStatusObservable();
                });
        }
    }
}
