namespace Buerokratt.Common.Models
{
    public static class ContentTypes
    {
        /// <summary>
        /// Content type for a message specifying that a routing error has occurred.
        /// </summary>
        public const string Error = "application/vnd.dmr.error+json;version=1";

        /// <summary>
        /// Content type for requesting classification.
        /// </summary>
        public const string ClassificationRequest = "application/vnd.classifier.classification+json;version=1";

        /// <summary>
        /// Content type for routing a message via the DMR.
        /// </summary>
        public const string MessageRequest = "application/vnd.dmr.messagerequest+json;version=1";

        /// <summary>
        /// Content type for indicating acknowledgement of receiving a message. Used by bots only. This content
        /// type is to indicate that the message in the payload is not one that requires any kind of reply.
        /// </summary>
        public const string MessageAcknowledgement = "application/vnd.dmr.messageacknowledge+json;version=1";
    }
}
