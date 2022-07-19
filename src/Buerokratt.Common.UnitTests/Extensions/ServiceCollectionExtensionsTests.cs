using Buerokratt.Common.AsyncProcessor;
using Buerokratt.Common.CentOps;
using Buerokratt.Common.CentOps.Interfaces;
using Buerokratt.Common.Dmr;
using Buerokratt.Common.Dmr.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Xunit;

namespace Buerokratt.Common.UnitTests.Extensions
{
    public sealed class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddDmrServiceThrowsForMissingDmrSettings()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddDmrService(null, new CentOpsServiceSettings()));
        }

        [Fact]
        public void AddDmrServiceThrowsForMissingCentOpsSettings()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddDmrService(new DmrServiceSettings(), null));
        }

        [Fact]
        public void AddDmrServiceAddsServices()
        {
            //Arrange
            DmrServiceSettings dmrSettings = new()
            {
                ClientName = "Foo",
            };

            CentOpsServiceSettings centOpsSettings = new()
            {
                CentOpsUri = new Uri("https://centOps"),
                CentOpsApiKey = "APIKEY"
            };

            var collection = new ServiceCollection();

            // Act
            collection.AddDmrService(dmrSettings, centOpsSettings);

            // Assert
            var dmrService = collection.First(e => e.ServiceType == typeof(IAsyncProcessorService<DmrRequest>));
            var dmrServiceSettings = collection.First(e => e.ServiceType == typeof(DmrServiceSettings));
            var dmrHostedService = collection.First(e => e.ServiceType == typeof(IHostedService) && e.ImplementationType.Name.StartsWith("AsyncProcessorHostedService", StringComparison.InvariantCultureIgnoreCase));
            var dmrSettingsAsAsyncProcessor = collection.First(e => e.ServiceType == typeof(AsyncProcessorSettings));

            var participantPollerHostedService = collection.First(e => e.ServiceType == typeof(IHostedService) && e.ImplementationType.Name == "ParticipantPoller");
            var centOpsServiceSettings = collection.First(e => e.ServiceType == typeof(CentOpsServiceSettings));
            var centOpsService = collection.First(e => e.ServiceType == typeof(ICentOpsService));
            var concurrentDictionary = collection.First(e => e.ServiceType.Name.Contains("ConcurrentDictionary", StringComparison.InvariantCultureIgnoreCase));

            Assert.Equal(ServiceLifetime.Singleton, dmrService.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, dmrServiceSettings.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, dmrSettingsAsAsyncProcessor.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, dmrHostedService.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, participantPollerHostedService.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, centOpsServiceSettings.Lifetime);
            Assert.Equal(ServiceLifetime.Transient, centOpsService.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, concurrentDictionary.Lifetime);
        }
    }
}
