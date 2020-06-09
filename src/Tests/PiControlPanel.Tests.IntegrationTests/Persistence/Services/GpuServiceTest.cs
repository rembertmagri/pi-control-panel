namespace PiControlPanel.Tests.IntegrationTests.Persistence.Services
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Moq;
    using NLog;
    using PiControlPanel.Infrastructure.Persistence.Entities;
    using PiControlPanel.Infrastructure.Persistence.MapperProfile;
    using PiControlPanel.Infrastructure.Persistence.Repositories;
    using PiControlPanel.Infrastructure.Persistence.Services;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class GpuServiceTest
    {
        [Fact]
        public async void GetAsync_EmptyDatabase_NoRecordFound()
        {
            var connection = PersistenceTestingHelper.SetupSqLiteDb();
            try
            {
                // Arrange
                var logger = new Mock<ILogger>().Object;
                var mapper = new AutoMapperConfiguration().GetIMapper();

                // using a different to make sure data is fetched from the db and not coming from the contextToPopulateData
                using (var databaseContext = PersistenceTestingHelper.CreateDbContextForSqLite(connection))
                {
                    var uow = new UnitOfWork(databaseContext, logger);
                    var service = new GpuService(uow, mapper, logger);

                    // Act
                    var result = await service.GetAsync();

                    // Assert
                    result.Should().BeNull();
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async void GetAsync_EntityInDatabase_RecordFoundWithCorrectValues()
        {
            var connection = PersistenceTestingHelper.SetupSqLiteDb();
            try
            {
                // Arrange
                var frequency = 1500;
                var memory = 4000;
                using (var contextToPopulateData = PersistenceTestingHelper.CreateDbContextForSqLite(connection))
                {
                    var entity = new Gpu()
                    {
                        ID = 1,
                        Frequency = frequency,
                        Memory = memory
                    };
                    contextToPopulateData.Gpu.Add(entity);
                    contextToPopulateData.SaveChanges();
                }

                var logger = new Mock<ILogger>().Object;
                var mapper = new AutoMapperConfiguration().GetIMapper();

                // using a different to make sure data is fetched from the db and not coming from the contextToPopulateData
                using (var databaseContext = PersistenceTestingHelper.CreateDbContextForSqLite(connection))
                {
                    var uow = new UnitOfWork(databaseContext, logger);
                    var service = new GpuService(uow, mapper, logger);

                    // Act
                    var result = await service.GetAsync();

                    // Assert
                    result.Should().NotBeNull();
                    result.Frequency.Should().Be(frequency);
                    result.Memory.Should().Be(memory);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
