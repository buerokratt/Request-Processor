using System.Diagnostics.CodeAnalysis;

namespace Buerokratt.Common.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class Message
    {
        public string? Payload { get; set; }
        public HeadersInput? Headers { get; set; }
    }
}