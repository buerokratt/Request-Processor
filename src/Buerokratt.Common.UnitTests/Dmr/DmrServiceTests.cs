using Buerokratt.Common.CentOps.Interfaces;
using Buerokratt.Common.CentOps.Models;
using Buerokratt.Common.Dmr;
using Buerokratt.Common.Models;
using Buerokratt.Common.UnitTests.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Buerokratt.Common.UnitTests.Dmr
{
    public sealed class DmrServiceTests : IDisposable
    {
        private readonly MockHttpMessageHandler httpMessageHandler = new();
        private readonly Mock<ILogger<DmrService>> logger = new();

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
            _ = httpMessageHandler.SetupWithExpectedMessage();

            var centOps = ConfigureCentOps();
            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new DmrService(centOps, clientFactory.Object, new DmrServiceSettings(), logger.Object);

            sut.Enqueue(GetDmrRequest());

            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldCallDmrApiForEachGivenRequestWhenMultipleRequestsAreRecorded()
        {
            var centOps = ConfigureCentOps();
            _ = httpMessageHandler
                .SetupWithExpectedMessage("my first message", "education")
                .SetupWithExpectedMessage("my second message", "social");

            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new DmrService(centOps, clientFactory.Object, new DmrServiceSettings(), logger.Object);

            sut.Enqueue(GetDmrRequest("my first message", "education"));
            sut.Enqueue(GetDmrRequest("my second message", "social"));

            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldNotThrowExceptionWhenCallToDmrApiErrors()
        {
            var centOps = ConfigureCentOps();
            using var dmrHttpClient = new MockHttpMessageHandler();
            _ = dmrHttpClient.When("/").Respond(HttpStatusCode.BadGateway);

            var clientFactory = GetHttpClientFactory(dmrHttpClient);

            var sut = new DmrService(centOps, clientFactory.Object, new DmrServiceSettings(), logger.Object);

            sut.Enqueue(GetDmrRequest());

            await sut.ProcessRequestsAsync().ConfigureAwait(false);
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
            httpMessageHandler.Dispose();
        }
    }
}
