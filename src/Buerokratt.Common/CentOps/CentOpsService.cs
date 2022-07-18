using Buerokratt.Common.CentOps.Interfaces;
using Buerokratt.Common.CentOps.Models;
using System.Collections.Concurrent;

namespace Buerokratt.Common.CentOps
{
    public class CentOpsService : ICentOpsService
    {
        private readonly ConcurrentDictionary<string, Participant> _participants;

        public CentOpsService(ConcurrentDictionary<string, Participant> participants)
        {
            this._participants = participants ?? throw new ArgumentNullException(nameof(participants));
        }

        public Task<Uri?> FetchEndpointByName(string name)
        {
            return
                Task.FromResult(
                    _participants.ContainsKey(name) && !string.IsNullOrEmpty(_participants[name].Host)
                    ? new Uri(_participants[name].Host!)
                    : null);
        }

        public Task<IEnumerable<Participant>> FetchParticipantsByType(ParticipantType type)
        {
            return Task.FromResult(_participants.Values.Where(p => p.Type == type));
        }
    }
}
