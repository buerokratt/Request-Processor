using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RequestProcessor.AsyncProcessor;
using RequestProcessor.Models;

namespace RequestProcessor.UnitTests
{
    public class APSTestImplementation : AsyncProcessorService<Message, AsyncProcessorSettings>
    {
        public IList<Message> messages = new List<Message>();
        
        public APSTestImplementation(IHttpClientFactory httpClientFactory, AsyncProcessorSettings config, ILogger logger) : base(httpClientFactory, config, logger) {}

        public override Task ProcessRequestAsync(Message payload)
        {
            messages.Add(payload);

            return Task.CompletedTask;
        }
    }
}