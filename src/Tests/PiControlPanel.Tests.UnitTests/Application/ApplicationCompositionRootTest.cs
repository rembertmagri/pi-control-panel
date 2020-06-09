namespace PiControlPanel.Tests.UnitTests.Application
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using LightInject;
    using Moq;
    using PiControlPanel.Application.Services;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class ApplicationCompositionRootTest
    {
        [Fact]
        public void Compose_MockedServiceRegistry_ServiceRegistrationList()
        {
            // Arrange
            Mock<IServiceRegistry> serviceRegistry = new Mock<IServiceRegistry>();
            serviceRegistry.Setup(m => m.AvailableServices).Returns(new List<ServiceRegistration>());
            var applicationCompositionRoot = new ApplicationCompositionRoot();

            // Act
            applicationCompositionRoot.Compose(serviceRegistry.Object);

            // Assert
            serviceRegistry.Object.AvailableServices.Should().BeOfType<List<ServiceRegistration>>();
        }
    }
}
