using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RequestProcessor.Models
{
    public class HeadersInput
    {
        [Required]
        [FromHeader(Name = Constants.XSentByHeaderName)]
        public string XSentBy { get; set; }

        [Required]
        [FromHeader(Name = Constants.XSendToHeaderName)]
        public string XSendTo { get; set; }

        [Required]
        [FromHeader(Name = Constants.XMessageIdHeaderName)]
        public string XMessageId { get; set; }

        [FromHeader(Name = Constants.XMessageIdRefHeaderName)]
        public string XMessageIdRef { get; set; }

        [FromHeader(Name = Constants.XModelTypeHeaderName)]
        public string XModelType { get; set; }

        [FromHeader(Name = Constants.ContentTypeHeaderName)]
        public string ContentType { get; set; }
    }
}