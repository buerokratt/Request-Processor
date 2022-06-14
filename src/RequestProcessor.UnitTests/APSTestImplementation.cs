using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RequestProcessor.AsyncProcessor;
using RequestProcessor.Models;

namespace RequestProcessor.UnitTests
{
    public class ApsTestImplementation : AsyncProcessorService<Message, AsyncProcessorSettings>
    {
        public readonly IList<Message> Messages = new List<Message>();

        public ApsTestImplementation(IHttpClientFactory httpClientFactory, AsyncProcessorSettings config, ILogger logger) : base(httpClientFactory, config, logger) {}

        public override Task ProcessRequestAsync(Message payload)
        {
            Messages.Add(payload);

            return Task.CompletedTask;
        }
    }
}