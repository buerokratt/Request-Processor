﻿using Buerokratt.Common.CentOps.Interfaces;
using Buerokratt.Common.CentOps.Models;
using System.Collections.Concurrent;

namespace Buerokratt.Common.CentOps
{
    public class CentOpsService : ICentOpsService
    {
        private readonly ConcurrentDictionary<string, Participant> participants;

        public CentOpsService(ConcurrentDictionary<string, Participant> participants)
        {
            this.participants = participants ?? throw new ArgumentNullException(nameof(participants));
        }

        public Task<Uri?> FetchEndpointByName(string name)
        {
            return
                Task.FromResult(
                    participants.ContainsKey(name) && !string.IsNullOrEmpty(participants[name].Host)
                    ? new Uri(participants[name].Host!)
                    : null);
        }

        public Task<IEnumerable<Participant>> FetchParticipantsByType(ParticipantType type)
        {
            return Task.FromResult(participants.Values.Where(p => p.Type == type));
        }
    }
}