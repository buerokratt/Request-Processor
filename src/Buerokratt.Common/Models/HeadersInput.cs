using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Buerokratt.Common.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class HeadersInput
    {
        [Required]
        [FromHeader(Name = Headers.XSentByHeaderName)]
        public string? XSentBy { get; set; }

        [Required]
        [FromHeader(Name = Headers.XSendToHeaderName)]
        public string? XSendTo { get; set; }

        [Required]
        [FromHeader(Name = Headers.XMessageIdHeaderName)]
        public string? XMessageId { get; set; }

        [FromHeader(Name = Headers.XMessageIdRefHeaderName)]
        public string? XMessageIdRef { get; set; }

        [FromHeader(Name = Headers.XModelTypeHeaderName)]
        public string? XModelType { get; set; }

        [FromHeader(Name = Headers.ContentTypeHeaderName)]
        public string? ContentType { get; set; }
    }
}