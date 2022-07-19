using Buerokratt.Common.AsyncProcessor;
using Buerokratt.Common.Dmr.Extensions;
using Buerokratt.Common.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Buerokratt.Common.Dmr
{
    /// <summary>
    /// A service that handles calls to the DMR API
    /// </summary>
    public class DmrService : AsyncProcessorService<DmrRequest, DmrServiceSettings>
    {
        public DmrService(
            IHttpClientFactory httpClientFactory,
            DmrServiceSettings config,
            ILogger<DmrService> logger) :
                base(httpClientFactory, config, logger)
        {
        }

        public override async Task ProcessRequestAsync(DmrRequest payload)
        {
            if (payload == null || payload.Headers == null || payload.Payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            try
            {
                // Setup message
                using var requestMessage = CreateRequestMessage(payload);

                // Send request
                var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var errorReason = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new HttpRequestException(errorReason);
                }

                Logger.DmrCallback(payload.Payload?.Classification ?? string.Empty, payload.Payload?.Message ?? string.Empty);
            }
            catch (HttpRequestException exception)
            {
                Logger.DmrCallbackFailed(exception);
            }
        }

        private static string EncodeBase64(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            byte[] bytes = Encoding.UTF8.GetBytes(content);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        private static HttpRequestMessage CreateRequestMessage(DmrRequest request)
        {
            var jsonPayload = JsonSerializer.Serialize(request.Payload);
            var jsonPayloadBase64 = EncodeBase64(jsonPayload);
            var content = new StringContent(
                jsonPayloadBase64,
                Encoding.UTF8,
                MediaTypeNames.Text.Plain);

            var requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = content,
            };

            requestMessage.Headers.Add(HeaderNames.XMessageIdHeaderName, request.Headers.XMessageId);
            requestMessage.Headers.Add(HeaderNames.XMessageIdRefHeaderName, request.Headers.XMessageIdRef);
            requestMessage.Headers.Add(HeaderNames.XSendToHeaderName, request.Headers.XSendTo);
            requestMessage.Headers.Add(HeaderNames.XSentByHeaderName, request.Headers.XSentBy);
            requestMessage.Headers.Add(HeaderNames.XModelTypeHeaderName, request.Headers.XModelType);

            // Unless specified by the caller - use the text/plain mime type.
            _ = content.Headers.ContentType = MediaTypeHeaderValue.Parse(request.Headers.ContentType ?? MediaTypeNames.Text.Plain);

            return requestMessage;
        }
    }
}