using Buerokratt.Common.AsyncProcessor;
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
            DmrServiceSettings DefaultServiceConfig = new()
            {
                DmrApiUri = new Uri("https://dmr.fakeurl.com"),
                ClientName = "Foo",
            };
            var collection = new ServiceCollection();

            // Act
            collection.AddDmrService(DefaultServiceConfig);
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
