using Buerokratt.Common.CentOps.Interfaces;
using Buerokratt.Common.CentOps.Models;
using Buerokratt.Common.Dmr;
using Buerokratt.Common.Models;
using Buerokratt.Common.UnitTests.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Buerokratt.Common.UnitTests.Dmr
{
    public sealed class DmrServiceTests : IDisposable
    {
        private readonly MockHttpMessageHandler _httpMessageHandler = new();
        private readonly Mock<ILogger<DmrService>> _logger;

        public DmrServiceTests()
        {
            _logger = new Mock<ILogger<DmrService>>();
            _ = _logger
                .Setup(m => m.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true);
        }

        private static ICentOpsService ConfigureCentOps()
        {
            var centOpsMock = new Mock<ICentOpsService>();
            _ = centOpsMock.Setup(x => x.FetchParticipantsByType(ParticipantType.Dmr))
                       .ReturnsAsync(new[]
                       {
                           new Participant
                           {
                               Host = "https://dmr.fakeurl.com",
                               Name = "Dmr",
                               Type = ParticipantType.Dmr,
                               Id = "1"
                           }
                       });

            return centOpsMock.Object;
        }

        [Fact]
        public async Task ShouldCallDmrApiWithGivenRequestWhenRequestIsRecorded()
        {
            // Arrange
            _ = _httpMessageHandler.SetupWithExpectedMessage();

            var centOps = ConfigureCentOps();
            var clientFactory = GetHttpClientFactory(_httpMessageHandler);

            var sut = new DmrService(centOps, clientFactory.Object, new DmrServiceSettings(), _logger.Object);

            sut.Enqueue(GetDmrRequest());

            // Act
            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            // Assert
            _httpMessageHandler.VerifyNoOutstandingExpectation();

            _logger.Verify(
               m => m.Log(
                   LogLevel.Information,
                   It.Is<EventId>(e => e.Id == 1 && e.Name == "DmrCallbackPosted"),
                   It.Is<It.IsAnyType>((v, t) => true),
                   It.IsAny<Exception>(),
                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once());
        }

        [Fact]
        public async Task ShouldCallDmrApiForEachGivenRequestWhenMultipleRequestsAreRecorded()
        {
            // A
            var centOps = ConfigureCentOps();
            _ = _httpMessageHandler
                .SetupWithExpectedMessage("my first message", "education")
                .SetupWithExpectedMessage("my second message", "social");

            var clientFactory = GetHttpClientFactory(_httpMessageHandler);

            var sut = new DmrService(centOps, clientFactory.Object, new DmrServiceSettings(), _logger.Object);

            sut.Enqueue(GetDmrRequest("my first message", "education"));
            sut.Enqueue(GetDmrRequest("my second message", "social"));

            // Act
            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            // Assert
            _httpMessageHandler.VerifyNoOutstandingExpectation();

            _logger.Verify(
               m => m.Log(
                   LogLevel.Information,
                   It.Is<EventId>(e => e.Id == 1 && e.Name == "DmrCallbackPosted"),
                   It.Is<It.IsAnyType>((v, t) => true),
                   It.IsAny<Exception>(),
                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task ShouldNotThrowExceptionWhenCallToDmrApiErrors()
        {
            // Arrange
            var centOps = ConfigureCentOps();
            using var dmrHttpClient = new MockHttpMessageHandler();
            _ = dmrHttpClient.When("/").Respond(HttpStatusCode.BadGateway);

            var clientFactory = GetHttpClientFactory(dmrHttpClient);

            var sut = new DmrService(centOps, clientFactory.Object, new DmrServiceSettings(), _logger.Object);

            sut.Enqueue(GetDmrRequest());

            // Act
            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            // Assert
            _logger.Verify(
                m => m.Log(
                    LogLevel.Error,
                    It.Is<EventId>(e => e.Id == 2 && e.Name == "DmrCallbackFailed"),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<HttpRequestException>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Fact]
        public async Task ShouldThrowAndLogForMissingDmrInCentOps()
        {
            // Arrange
            var centOpsMock = new Mock<ICentOpsService>();
            _ = centOpsMock.Setup(c => c.FetchParticipantsByType(ParticipantType.Dmr)).ReturnsAsync(Enumerable.Empty<Participant>());

            using var dmrHttpClient = new MockHttpMessageHandler();
            _ = dmrHttpClient.When("/").Respond(HttpStatusCode.BadGateway);

            var clientFactory = GetHttpClientFactory(dmrHttpClient);

            var sut = new DmrService(centOpsMock.Object, clientFactory.Object, new DmrServiceSettings(), _logger.Object);

            sut.Enqueue(GetDmrRequest());

            // Act
            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            // Assert
            _logger.Verify(
                m => m.Log(
                    LogLevel.Error,
                    It.Is<EventId>(e => e.Id == 2 && e.Name == "DmrCallbackFailed"),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.Is<Exception>(ex => ex.Message == "No active DMRs found."),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        private static Mock<IHttpClientFactory> GetHttpClientFactory(MockHttpMessageHandler messageHandler)
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _ = mockHttpClientFactory
                .Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                {
                    var client = messageHandler.ToHttpClient();
                    return client;
                });

            return mockHttpClientFactory;
        }


        private static DmrRequest GetDmrRequest(string message = "my test message", string classification = "border")
        {
            var headers = new HeadersInput
            {
                XSentBy = "MockClassifier.UnitTests.Services.Dmr.DmrServiceTests",
                XSendTo = Constants.ClassifierId,
                XMessageId = "1f7b356d-a6f4-4aeb-85cd-9d570dbc7606",
                XMessageIdRef = "5822c6ef-177d-4dd7-b4c5-0d9d8c8d2c35",
                XModelType = "application/vnd.classifier.classification+json;version=1",
                ContentType = "text/plain"
            };

            var request = new DmrRequest(headers)
            {
                Payload = new DmrRequestPayload
                {
                    Message = message,
                    Classification = classification
                }
            };

            return request;
        }

        public void Dispose()
        {
            _httpMessageHandler.Dispose();
        }
    }
}
