using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Buerokratt.Common.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class HeadersInput
    {
        [Required]
        [FromHeader(Name = HeaderNames.XSentByHeaderName)]
        public string? XSentBy { get; set; }

        [Required]
        [FromHeader(Name = HeaderNames.XSendToHeaderName)]
        public string? XSendTo { get; set; }

        [Required]
        [FromHeader(Name = HeaderNames.XMessageIdHeaderName)]
        public string? XMessageId { get; set; }

        [FromHeader(Name = HeaderNames.XMessageIdRefHeaderName)]
        public string? XMessageIdRef { get; set; }

        [FromHeader(Name = HeaderNames.XModelTypeHeaderName)]
        public string? XModelType { get; set; }

        [FromHeader(Name = HeaderNames.ContentTypeHeaderName)]
        public string? ContentType { get; set; }
    }
}