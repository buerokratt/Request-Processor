using Buerokratt.Common.CentOps;
using Buerokratt.Common.CentOps.Interfaces;
using Buerokratt.Common.CentOps.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Buerokratt.Common.Utils
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Helper extension to configure IOC container with <see cref="ParticipantPoller"/> Service and associated services.
        /// </summary>
        /// <param name="services">The services collection that <see cref="ParticipantPoller"/> and related services will be added to.</param>
        /// <param name="settings">A settings object for the <see cref="ParticipantPoller"/></param>
        public static void AddParticipantPoller(this IServiceCollection services, CentOpsServiceSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _ = services.AddSingleton(settings);
            _ = services.AddSingleton<ConcurrentDictionary<string, Participant>>();
            _ = services.AddTransient<ICentOpsService, CentOpsService>();
            _ = services.AddHostedService<ParticipantPoller>();
        }
    }
}
