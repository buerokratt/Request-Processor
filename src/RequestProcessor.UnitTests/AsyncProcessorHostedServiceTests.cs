using System;
using RequestProcessor.Models;
using RequestProcessor.AsyncProcessor;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RequestProcessor.UnitTests
{
    public class AsyncProcessorHostedServiceTests
    {
        [Fact]
        public async Task AsyncProcessorHostedServiceCanStartAsync()
        {
            // Arrange
            var processor = new Mock<IAsyncProcessorService<Message>>();
            var logger = new Mock<ILogger<AsyncProcessorHostedService<Message>>>();
            _ = logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            using var sut =
                new AsyncProcessorHostedService<Message>(
                    processor.Object,
                    new AsyncProcessorSettings() { RequestProcessIntervalMs = 0 },
                    logger.Object);

            var cancellationToken = new CancellationToken();

            // Act
            await sut.StartAsync(cancellationToken).ConfigureAwait(true);

            // Assert
            Assert.True(sut.IsRunning);
            await Task.Delay(1000).ConfigureAwait(true);
            await sut.StopAsync(cancellationToken).ConfigureAwait(true);

            processor.Verify(p => p.ProcessRequestsAsync(), Times.AtLeastOnce);
            Assert.False(sut.IsRunning);
            logger.Verify(x => x.Log(
                LogLevel.Information,
                new EventId(11, "AsyncProcessorStateChange"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task AsyncProcessorHostedServiceWillStopAsync()
        {
            // Arrange
            var processor = new Mock<IAsyncProcessorService<Message>>();
            var logger = new Mock<ILogger<AsyncProcessorHostedService<Message>>>();
            _ = logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            using var sut =
                new AsyncProcessorHostedService<Message>(
                    processor.Object,
                    new AsyncProcessorSettings() { RequestProcessIntervalMs = 0 },
                    logger.Object);

            var cancellationToken = new CancellationToken();

            await sut.StartAsync(cancellationToken).ConfigureAwait(true);

            await Task.Delay(1000).ConfigureAwait(true);

            // Act
            await sut.StopAsync(cancellationToken).ConfigureAwait(true);

            // Allow time to stop.
            await Task.Delay(500).ConfigureAwait(true);

            processor.Reset();

            await Task.Delay(500).ConfigureAwait(true);

            // Assert
            processor.Verify(p => p.ProcessRequestsAsync(), Times.Never);
            Assert.False(sut.IsRunning);
            logger.Verify(x => x.Log(
                LogLevel.Information,
                new EventId(11, "AsyncProcessorStateChange"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }
    }
}
