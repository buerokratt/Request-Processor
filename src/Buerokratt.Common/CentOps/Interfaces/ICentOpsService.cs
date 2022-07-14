using Buerokratt.Common.CentOps.Models;

namespace Buerokratt.Common.CentOps.Interfaces
{
    /// <summary>
    /// Interface which describes CentOps functionality.
    /// </summary>
    public interface ICentOpsService
    {
        Task<Uri?> FetchEndpointByName(string name);

        Task<IEnumerable<Participant>> FetchParticipantsByType(ParticipantType type);
    }
}
