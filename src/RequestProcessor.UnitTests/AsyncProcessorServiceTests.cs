using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using RequestProcessor.AsyncProcessor;
using RequestProcessor.Models;
using RichardSzalay.MockHttp;
using Xunit;

namespace RequestProcessor.UnitTests
{
    public class AsyncProcessorServiceTests
    {
        [Fact]
        public void AsyncProcessorImplementationThrowsForNullConfiguration()
        {
            Mock<ILogger<ApsTestImplementation>> logger = new();
            _ = Assert.Throws<ArgumentNullException>(() => new ApsTestImplementation(null, new AsyncProcessorSettings(), logger.Object));
        }

        [Fact]
        public void AsyncProcessorImplementationThrowsForNullClient()
        {
            Mock<ILogger<ApsTestImplementation>> logger = new();
            _ = Assert.Throws<ArgumentNullException>(() => new ApsTestImplementation(new Mock<IHttpClientFactory>().Object, null, logger.Object));
        }

        [Fact]
        public async Task EnsureMessageIsDequeued()
        {
            // Arrange
            Mock<ILogger<ApsTestImplementation>> logger = new();
            logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            var sut = new ApsTestImplementation(new Mock<IHttpClientFactory>().Object, new AsyncProcessorSettings(),
                logger.Object);

            // Act
            sut.Enqueue(new Message
            {
                Payload = "Test Data",
                Headers = new HeadersInput
                {
                    XSendTo = Constants.ClassifierId,
                    XSentBy = "Police",
                }
            });

            await sut.ProcessRequestsAsync().ConfigureAwait(true);

            // Assert
            Assert.Equal(1, sut.Messages.Count);
            logger.Verify(x => x.Log(
                LogLevel.Information,
                new EventId(12, "AsyncProcessorTelemetry"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<HttpRequestException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Fact]
        public async Task EnsureTelemetryNotLoggedIfNothingQueued()
        {
            // Arrange
            Mock<ILogger<ApsTestImplementation>> logger = new();
            logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            var sut = new ApsTestImplementation(new Mock<IHttpClientFactory>().Object, new AsyncProcessorSettings(),
                logger.Object);

            // Act
            await sut.ProcessRequestsAsync().ConfigureAwait(true);

            // Assert
            Assert.Equal(0, sut.Messages.Count);
            logger.Verify(x => x.Log(
                LogLevel.Information,
                new EventId(12, "AsyncProcessorTelemetry"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<HttpRequestException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Never);
        }

    }
}