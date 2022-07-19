using Microsoft.AspNetCore.Http;

namespace Buerokratt.Common.Models.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IHeaderDictionary"/>.
    /// </summary>
    public static class HeaderDictionaryExtensions
    {
        /// <summary>
        /// Extracts <see cref="HeadersInput"/> values from the given <see cref="IHeaderDictionary"/> object.
        /// </summary>
        public static HeadersInput ToHeadersInput(this IHeaderDictionary headers)
        {
            ArgumentNullException.ThrowIfNull(headers);

            return new HeadersInput
            {
                XSentBy = headers[Headers.XSentByHeaderName],
                XSendTo = headers[Headers.XSendToHeaderName],
                XMessageId = headers[Headers.XMessageIdHeaderName],
                XMessageIdRef = headers[Headers.XMessageIdHeaderName],
                XModelType = headers[Headers.XModelTypeHeaderName],
                ContentType = headers[Headers.ContentTypeHeaderName]
            };
        }
    }
}
