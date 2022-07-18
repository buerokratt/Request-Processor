using Buerokratt.Common.CentOps;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Buerokratt.Common.UnitTests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddParticipantPollerThrowsforNullSettings()
        {
            // Arrange
            var services = new ServiceCollection();

            //Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddParticipantPoller(services, null));
        }

        [Fact]
        public void AddParticipantPollerSettingsAdded()
        {
            // Arrange
            var services = new ServiceCollection();
            var settings = new CentOpsServiceSettings();

            // Act
            ServiceCollectionExtensions.AddParticipantPoller(services, settings);

            // Assert
            Assert.Contains(services, service => service.ServiceType.Name == "CentOpsServiceSettings");
            Assert.Contains(services, service => service.ServiceType.Name == "IHostedService");
            Assert.Contains(services, service => service.ServiceType.Name == "ICentOpsService");
            Assert.Contains(services, service => service.ServiceType.Name.Contains("ConcurrentDictionary", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
