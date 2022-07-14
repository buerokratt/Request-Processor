using Buerokratt.Common.Dmr;
using Buerokratt.Common.Encoder;
using Buerokratt.Common.Models;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace Buerokratt.Common.UnitTests.Extensions
{
    internal static class MockHttpMessageHandlerExtensions
    {
        public static MockHttpMessageHandler SetupWithExpectedMessage(
            this MockHttpMessageHandler handler,
            string expectedMessage = "my test message",
            string classification = "border")
        {
            var payload = new DmrRequestPayload
            {
                Message = expectedMessage,
                Classification = classification,
            };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var jsonPayloadBase64 = new EncodingService().EncodeBase64(jsonPayload);
            _ = handler
                .Expect("/")
                .WithContent(jsonPayloadBase64)
                .WithHeaders(Constants.XSentByHeaderName, "MockClassifier.UnitTests.Services.Dmr.DmrServiceTests")
                .WithHeaders(Constants.XSendToHeaderName, Constants.ClassifierId)
                .WithHeaders(Constants.XModelTypeHeaderName, "application/vnd.classifier.classification+json;version=1")
                .WithHeaders(Constants.XMessageIdHeaderName, "1f7b356d-a6f4-4aeb-85cd-9d570dbc7606")
                .WithHeaders(Constants.XMessageIdRefHeaderName, "5822c6ef-177d-4dd7-b4c5-0d9d8c8d2c35")
                .WithHeaders(Constants.ContentTypeHeaderName, "text/plain")
                .Respond(HttpStatusCode.Accepted);

            return handler;
        }
    }
}
