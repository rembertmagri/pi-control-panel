namespace PiControlPanel.Infrastructure.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    /// <inheritdoc/>
    public class ControlPanelDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelDbContext"/> class.
        /// </summary>
        /// <param name="configuration">The IConfiguration instance.</param>
        public ControlPanelDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.Database.EnsureCreated();
        }

        /// <summary>
        /// Gets or sets the database set for the Chipset entity.
        /// </summary>
        public DbSet<Entities.Chipset> Chipset { get; set; }

        /// <summary>
        /// Gets or sets the database set for the Cpu entity.
        /// </summary>
        public DbSet<Entities.Cpu.Cpu> Cpu { get; set; }

        /// <summary>
        /// Gets or sets the database set for the CpuFrequency entity.
        /// </summary>
        public DbSet<Entities.Cpu.CpuFrequency> CpuFrequency { get; set; }

        /// <summary>
        /// Gets or sets the database set for the CpuTemperature entity.
        /// </summary>
        public DbSet<Entities.Cpu.CpuTemperature> CpuTemperature { get; set; }

        /// <summary>
        /// Gets or sets the database set for the CpuLoadStatus entity.
        /// </summary>
        public DbSet<Entities.Cpu.CpuLoadStatus> CpuLoadStatus { get; set; }

        /// <summary>
        /// Gets or sets the database set for the CpuProcess entity.
        /// </summary>
        public DbSet<Entities.Cpu.CpuProcess> CpuProcess { get; set; }

        /// <summary>
        /// Gets or sets the database set for the Gpu entity.
        /// </summary>
        public DbSet<Entities.Gpu> Gpu { get; set; }

        /// <summary>
        /// Gets or sets the database set for the Os entity.
        /// </summary>
        public DbSet<Entities.Os.Os> Os { get; set; }

        /// <summary>
        /// Gets or sets the database set for the OsStatus entity.
        /// </summary>
        public DbSet<Entities.Os.OsStatus> OsStatus { get; set; }

        /// <summary>
        /// Gets or sets the database set for the Disk entity.
        /// </summary>
        public DbSet<Entities.Disk.Disk> Disk { get; set; }

        /// <summary>
        /// Gets or sets the database set for the FileSystemStatus entity.
        /// </summary>
        public DbSet<Entities.Disk.FileSystemStatus> FileSystemStatus { get; set; }

        /// <summary>
        /// Gets or sets the database set for the RandomAccessMemory entity.
        /// </summary>
        public DbSet<Entities.Memory.RandomAccessMemory> RandomAccessMemory { get; set; }

        /// <summary>
        /// Gets or sets the database set for the RandomAccessMemoryStatus entity.
        /// </summary>
        public DbSet<Entities.Memory.RandomAccessMemoryStatus> RandomAccessMemoryStatus { get; set; }

        /// <summary>
        /// Gets or sets the database set for the SwapMemory entity.
        /// </summary>
        public DbSet<Entities.Memory.SwapMemory> SwapMemory { get; set; }

        /// <summary>
        /// Gets or sets the database set for the SwapMemoryStatus entity.
        /// </summary>
        public DbSet<Entities.Memory.SwapMemoryStatus> SwapMemoryStatus { get; set; }

        /// <summary>
        /// Gets or sets the database set for the Network entity.
        /// </summary>
        public DbSet<Entities.Network.Network> Network { get; set; }

        /// <summary>
        /// Gets or sets the database set for the NetworkInterfaceStatus entity.
        /// </summary>
        public DbSet<Entities.Network.NetworkInterfaceStatus> NetworkInterfaceStatus { get; set; }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = this.configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlite(connectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
        }
    }
}
