using Buerokratt.Common.AsyncProcessor;
using Buerokratt.Common.CentOps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Buerokratt.Common.Dmr.Extensions
{
    /// <summary>
    /// Extension class to help add all services related to the DMR
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Helper extension to easily install the DMR Service
        /// </summary>
        /// <param name="services">The services collection that <see cref="DmrService"/> and related services will be added to.</param>
        /// <param name="settings">A settings object for the <see cref="DmrService"/></param>
        public static void AddDmrService(this IServiceCollection services, DmrServiceSettings dmrSettings, CentOpsServiceSettings centOpsSettings)
        {
            if (dmrSettings == null)
            {
                throw new ArgumentNullException(nameof(dmrSettings));
            }

            _ = services.AddHttpClient(dmrSettings.ClientName, client =>
            {
                client.Timeout = TimeSpan.FromMilliseconds(dmrSettings.HttpRequestTimeoutMs);
            });

            if (centOpsSettings == null)
            {
                throw new ArgumentNullException(nameof(centOpsSettings));
            }

            // CentOps and participant poller now required for Dmr calls to succeed.
            services.AddParticipantPoller(centOpsSettings);

            services.TryAddSingleton(dmrSettings);
            services.TryAddSingleton(dmrSettings as AsyncProcessorSettings);
            services.TryAddSingleton<IAsyncProcessorService<DmrRequest>, DmrService>();
            _ = services.AddHostedService<AsyncProcessorHostedService<DmrRequest>>();

        }
    }
}