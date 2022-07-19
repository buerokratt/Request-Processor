using Buerokratt.Common.Dmr.Extensions;
using Microsoft.Extensions.Logging;
using MockLogging;
using System;
using Xunit;

namespace Buerokratt.Common.UnitTests.Extensions
{
    public class LoggerExtensionsTests
    {
        private readonly MockLogger _mockLogger = new();

        [Theory]
        [InlineData("border", "message to border")]
        [InlineData("education", "message to education")]
        public void DmrCallbackShouldLogClassificationAndMessage(string classification, string message)
        {
            // Act
            _mockLogger.DmrCallbackSucceeded(classification, message);

            // Assert
            var entry = _mockLogger.VerifyLogEntry();
            _ = entry.HasEventId(new EventId(1, nameof(Common.Dmr.Extensions.LoggerExtensions.DmrCallbackSucceeded)))
                     .HasLogLevel(LogLevel.Information);
        }

        [Fact]
        public void DmrCallbackFailedShouldLogException()
        {
            // Arrange
            var exception = new InvalidOperationException("my test exception");

            // Act
            _mockLogger.DmrCallbackFailed(exception);

            // Assert
            var entry = _mockLogger.VerifyLogEntry();
            _ = entry.HasEventId(new EventId(2, nameof(Common.Dmr.Extensions.LoggerExtensions.DmrCallbackFailed)))
                     .HasExceptionOfType<InvalidOperationException>()
                     .HasLogLevel(LogLevel.Error);

            Assert.Equal(exception.Message, entry.Exception.Message);
        }
    }
}
