namespace PiControlPanel.Application.Services
{
    using System.Collections.Generic;
    using System.Reactive.Subjects;
    using Microsoft.Extensions.DependencyInjection;
    using PiControlPanel.Domain.Models.Hardware.Cpu;
    using PiControlPanel.Domain.Models.Hardware.Disk;
    using PiControlPanel.Domain.Models.Hardware.Memory;
    using PiControlPanel.Infrastructure.Persistence.Contracts.Repositories;
    using PiControlPanel.Infrastructure.Persistence.Repositories;
    using AutoMapper;
    using PiControlPanel.Infrastructure.Persistence.MapperProfile;
    using PiControlPanel.Domain.Models.Hardware.Os;
    using PiControlPanel.Domain.Models.Hardware.Network;
    using Contracts = PiControlPanel.Domain.Contracts.Infrastructure;
    using OnDemand = PiControlPanel.Infrastructure.OnDemand.Services;
    using Persistence = PiControlPanel.Infrastructure.Persistence.Services;

    /// <summary>
    ///     Contains extension methods for Infrastructure layer services.
    /// </summary>
    public static class InfraCustomServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds references to all infrastrucutre layer services.
        /// </summary>
        /// <param name="services">The service registry.</param>
        /// <returns>The altered service container.</returns>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddScoped<Contracts.Persistence.IChipsetService, Persistence.ChipsetService>();
            services.AddScoped<Contracts.Persistence.Cpu.ICpuService, Persistence.Cpu.CpuService>();
            services.AddScoped<Contracts.Persistence.Cpu.ICpuFrequencyService, Persistence.Cpu.CpuFrequencyService>();
            services.AddScoped<Contracts.Persistence.Cpu.ICpuSensorsStatusService, Persistence.Cpu.CpuSensorsStatusService>();
            services.AddScoped<Contracts.Persistence.Cpu.ICpuLoadStatusService, Persistence.Cpu.CpuLoadStatusService>();
            services.AddScoped<Contracts.Persistence.IGpuService, Persistence.GpuService>();
            services.AddScoped<Contracts.Persistence.Os.IOsService, Persistence.Os.OsService>();
            services.AddScoped<Contracts.Persistence.Os.IOsStatusService, Persistence.Os.OsStatusService>();
            services.AddScoped<Contracts.Persistence.Disk.IDiskService, Persistence.Disk.DiskService>();
            services.AddScoped<Contracts.Persistence.Disk.IFileSystemStatusService, Persistence.Disk.FileSystemStatusService>();
            services.AddScoped<Contracts.Persistence.Memory.IMemoryService<RandomAccessMemory>,
                Persistence.Memory.RandomAccessMemoryService>();
            services.AddScoped<Contracts.Persistence.Memory.IMemoryStatusService<RandomAccessMemoryStatus>,
                Persistence.Memory.RandomAccessMemoryStatusService>();
            services.AddScoped<Contracts.Persistence.Memory.IMemoryService<SwapMemory>,
                Persistence.Memory.SwapMemoryService>();
            services.AddScoped<Contracts.Persistence.Memory.IMemoryStatusService<SwapMemoryStatus>,
                Persistence.Memory.SwapMemoryStatusService>();
            services.AddScoped<Contracts.Persistence.Network.INetworkService, Persistence.Network.NetworkService>();
            services.AddScoped<Contracts.Persistence.Network.INetworkInterfaceStatusService, Persistence.Network.NetworkInterfaceStatusService>();

            services.AddScoped<Contracts.OnDemand.IControlPanelService, OnDemand.ControlPanelService>();
            services.AddScoped<Contracts.OnDemand.IUserAccountService, OnDemand.UserAccountService>();
            services.AddScoped<Contracts.OnDemand.IChipsetService, OnDemand.ChipsetService>();
            services.AddScoped<Contracts.OnDemand.ICpuService, OnDemand.CpuService>();
            services.AddScoped<Contracts.OnDemand.IMemoryService<RandomAccessMemory, RandomAccessMemoryStatus>,
                OnDemand.MemoryService<RandomAccessMemory, RandomAccessMemoryStatus>>();
            services.AddScoped<Contracts.OnDemand.IMemoryService<SwapMemory, SwapMemoryStatus>,
                OnDemand.MemoryService<SwapMemory, SwapMemoryStatus>>();
            services.AddScoped<Contracts.OnDemand.IGpuService, OnDemand.GpuService>();
            services.AddScoped<Contracts.OnDemand.IDiskService, OnDemand.DiskService>();
            services.AddScoped<Contracts.OnDemand.IOsService, OnDemand.OsService>();
            services.AddScoped<Contracts.OnDemand.INetworkService, OnDemand.NetworkService>();

            services.AddSingleton<IMapper>(factory => new AutoMapperConfiguration().GetIMapper());

            services.AddSingleton<ISubject<CpuFrequency>>(factory => new ReplaySubject<CpuFrequency>(1));
            services.AddSingleton<ISubject<CpuSensorsStatus>>(factory => new ReplaySubject<CpuSensorsStatus>(1));
            services.AddSingleton<ISubject<CpuLoadStatus>>(factory => new ReplaySubject<CpuLoadStatus>(1));
            services.AddSingleton<ISubject<RandomAccessMemoryStatus>>(factory => new ReplaySubject<RandomAccessMemoryStatus>(1));
            services.AddSingleton<ISubject<SwapMemoryStatus>>(factory => new ReplaySubject<SwapMemoryStatus>(1));
            services.AddSingleton<ISubject<IList<FileSystemStatus>>>(factory => new ReplaySubject<IList<FileSystemStatus>>(1));
            services.AddSingleton<ISubject<OsStatus>>(factory => new ReplaySubject<OsStatus>(1));
            services.AddSingleton<ISubject<IList<NetworkInterfaceStatus>>>(factory => new ReplaySubject<IList<NetworkInterfaceStatus>>(1));

            return services;
        }
    }
}
