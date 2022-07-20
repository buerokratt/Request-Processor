using System.Diagnostics.CodeAnalysis;

namespace Buerokratt.Common.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public static class ParticipantIds
    {
        /// <summary>
        /// The Id of the classifier - specified when a message needs classification.
        /// </summary>
        public static readonly string ClassifierId = "Classifier";

        /// <summary>
        /// The Id of the Dmr - specified when a routing error has occurred.
        /// </summary>
        public static readonly string DmrId = "Dmr";
    }
}
