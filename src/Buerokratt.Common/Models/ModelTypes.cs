namespace Buerokratt.Common.Models
{
    public static class ModelTypes
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
    }
}
