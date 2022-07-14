using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Buerokratt.Common.AsyncProcessor;
using Buerokratt.Common.Models;
using Microsoft.Extensions.Logging;

namespace Buerokratt.Common.UnitTests
{
    public class ApsTestImplementation : AsyncProcessorService<Message, AsyncProcessorSettings>
    {
        public IList<Message> Messages { get; } = new List<Message>();

        public ApsTestImplementation(IHttpClientFactory httpClientFactory, AsyncProcessorSettings config, ILogger logger) : base(httpClientFactory, config, logger) { }

        public override Task ProcessRequestAsync(Message payload)
        {
            Messages.Add(payload);
            return Task.CompletedTask;
        }
    }
}