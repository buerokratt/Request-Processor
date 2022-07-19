using System.Diagnostics.CodeAnalysis;

namespace Buerokratt.Common.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    internal static class Headers
    {
        /// <summary>
        /// Specifies the participant this associated message is intended for.
        /// </summary>
        internal const string XSentByHeaderName = "X-Sent-By";

        /// <summary>
        /// Specifies the participant the associated message originated from.
        /// </summary>
        internal const string XSendToHeaderName = "X-Send-To";

        /// <summary>
        /// Specified by the client - a unique value which specifies the payload.
        /// </summary>
        internal const string XMessageIdHeaderName = "X-Message-Id";

        /// <summary>
        /// Specified by the recipient service - value indicates the message Id this payload is in response to.
        /// e.g. application/vnd.classifier;version=1
        /// </summary>
        internal const string XMessageIdRefHeaderName = "X-Message-Id-Ref";

        /// <summary>
        /// Specifies the content type of the message as understood by Buerokratt participants.
        /// </summary>
        internal const string XModelTypeHeaderName = "X-Model-Type";

        /// <summary>
        /// Specifies the content type of the message.
        /// </summary>
        internal const string ContentTypeHeaderName = "Content-Type";
    }
}
