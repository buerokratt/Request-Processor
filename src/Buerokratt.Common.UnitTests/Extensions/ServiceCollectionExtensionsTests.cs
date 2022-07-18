using Buerokratt.Common.AsyncProcessor;
using Buerokratt.Common.CentOps;
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
            var dmrService = collection.First(e => e.ServiceType == typeof(IAsyncProcessorService<DmrRequest>));
            var dmrServiceSettings = collection.First(e => e.ServiceType == typeof(DmrServiceSettings));
            var dmrHostedService = collection.First(e => e.ServiceType == typeof(IHostedService));

            // Assert
            Assert.Equal(ServiceLifetime.Singleton, dmrService.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, dmrServiceSettings.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, dmrHostedService.Lifetime);
        }
    }
}
