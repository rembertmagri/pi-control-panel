namespace PiControlPanel.Tests.IntegrationTests.Persistence
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using AutoMapper;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Moq;
    using PiControlPanel.Infrastructure.Persistence;

    [ExcludeFromCodeCoverage]
    public class PersistenceTestingHelper
    {
        public static SqliteConnection SetupSqLiteDb()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Create the schema in the database
                using (var databaseContextToCreateDb = CreateDbContextForSqLite(connection))
                {
                    databaseContextToCreateDb.Database.EnsureCreated();
                }
            }
            catch (Exception e)
            {
                connection.Close();
                throw e;
            }

            return connection;
        }

        public static ControlPanelDbContext CreateDbContextForSqLite(SqliteConnection connection)
        {
            return new ControlPanelDbContext(new DbContextOptionsBuilder()
                .UseLoggerFactory(GetLoggerFactory())
                .UseSqlite(connection)
                .Options);
        }

        public static ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                builder.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information));
            return serviceCollection.BuildServiceProvider()
                .GetService<ILoggerFactory>();
        }
    }
}
