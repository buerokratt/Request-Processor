using Buerokratt.Common.CentOps;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Buerokratt.Common.UnitTests.CentOps
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddParticipantPollerThrowsforNullSettings()
        {
            // Arrange
            var services = new ServiceCollection();

            //Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => services.AddParticipantPoller(null));
        }

        [Fact]
        public void AddParticipantPollerSettingsAdded()
        {
            // Arrange
            var services = new ServiceCollection();
            var settings = new CentOpsServiceSettings();

            // Act
            services.AddParticipantPoller(settings);

            // Assert
            Assert.Contains(services, service => service.ServiceType.Name == "CentOpsServiceSettings" && service.Lifetime == ServiceLifetime.Singleton);
            Assert.Contains(services, service => service.ServiceType.Name == "IHostedService" && service.Lifetime == ServiceLifetime.Singleton);
            Assert.Contains(services, service => service.ServiceType.Name == "ICentOpsService" && service.Lifetime == ServiceLifetime.Transient);
            Assert.Contains(services,
                service => service.ServiceType.Name.Contains("ConcurrentDictionary", StringComparison.InvariantCultureIgnoreCase)
                        && service.Lifetime == ServiceLifetime.Singleton);
        }
    }
}
