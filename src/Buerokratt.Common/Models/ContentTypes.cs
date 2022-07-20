namespace Buerokratt.Common.Models
{
    public static class ContentTypes
    {
        /// <summary>
        /// Content type for a message specifying that a routing error has occurred.
        /// </summary>
        public static readonly string Error = "application/vnd.dmr.error+json;version=1";

        /// <summary>
        /// Content type for requesting classification.
        /// </summary>
        public static readonly string ClassificationRequest = "application/vnd.classifier.classification+json;version=1";

        /// <summary>
        /// Content type for routing a message via the DMR.
        /// </summary>
        public static readonly string MessageRequest = "application/vnd.dmr.messagerequest+json;version=1";

        /// <summary>
        /// Content type for indicating acknowledgement of receiving a message. This content type is to indicate that the message in the payload is not one that requires any kind of reply.
        /// This is a type that is used by Mock Bots only. 
        /// </summary>
        public static readonly string MessageAcknowledgement = "application/vnd.mockbot.messageacknowledge+json;version=1";
    }
}
