using Buerokratt.Common.CentOps.Interfaces;
using Buerokratt.Common.CentOps.Models;
using Buerokratt.Common.Dmr;
using Buerokratt.Common.Models;
using Buerokratt.Common.UnitTests.Extensions;
using Microsoft.Extensions.Logging;
using MockLogging;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
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
        private MockLogger<DmrService> _mockLogger = new();

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

            var sut = new DmrService(centOps, clientFactory.Object, new DmrServiceSettings(), _mockLogger);
            sut.Enqueue(GetDmrRequest());

            // Act
            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            // Assert
            _httpMessageHandler.VerifyNoOutstandingExpectation();

            var entry = _mockLogger.VerifyLogEntry();
            _ = entry.HasEventId(new EventId(1, "DmrCallbackPosted"))
                     .HasLogLevel(LogLevel.Information);
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

            var sut = new DmrService(centOps, clientFactory.Object, new DmrServiceSettings(), _mockLogger);

            sut.Enqueue(GetDmrRequest("my first message", "education"));
            sut.Enqueue(GetDmrRequest("my second message", "social"));

            // Act
            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            // Assert
            _httpMessageHandler.VerifyNoOutstandingExpectation();

            var entry1 = _mockLogger.VerifyLogEntry();
            _ = entry1.HasEventId(new EventId(1, "DmrCallbackPosted"))
                     .HasLogLevel(LogLevel.Information);

            var entry2 = _mockLogger.VerifyLogEntry();
            _ = entry2.HasEventId(new EventId(1, "DmrCallbackPosted"))
                     .HasLogLevel(LogLevel.Information);
        }

        [Fact]
        public async Task ShouldNotThrowExceptionWhenCallToDmrApiErrors()
        {
            // Arrange
            var centOps = ConfigureCentOps();
            using var dmrHttpClient = new MockHttpMessageHandler();
            _ = dmrHttpClient.When("/").Respond(HttpStatusCode.BadGateway);

            var clientFactory = GetHttpClientFactory(dmrHttpClient);

            var sut = new DmrService(centOps, clientFactory.Object, new DmrServiceSettings(), _mockLogger);

            sut.Enqueue(GetDmrRequest());

            // Act
            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            // Assert
            var entry = _mockLogger.VerifyLogEntry();
            _ = entry.HasEventId(new EventId(2, "DmrCallbackFailed"))
                     .HasLogLevel(LogLevel.Error);
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

            var sut = new DmrService(centOpsMock.Object, clientFactory.Object, new DmrServiceSettings(), _mockLogger);

            sut.Enqueue(GetDmrRequest());

            // Act
            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            // Assert
            var entry = _mockLogger.VerifyLogEntry();
            _ = entry.HasEventId(new EventId(2, "DmrCallbackFailed"))
                     .HasExceptionOfType<KeyNotFoundException>()
                     .HasLogLevel(LogLevel.Error);

            Assert.Equal("No active DMRs found.", entry.Exception.Message);
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
