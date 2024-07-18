using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.NetworkInformation;

namespace HealthCheckAPI
{
    public class ICMPHealthCheck : IHealthCheck
    {
        private readonly string Host;
        private readonly int HealthyRountripTime;

        public ICMPHealthCheck(string host, int healthyRountripTime)
        {
            Host = host;
            HealthyRountripTime = healthyRountripTime;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken ancellationToken = default)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(Host);

                switch (reply.Status)
                {
                    case IPStatus.Success:
                        var msg = $"ICMP to {Host} took {reply.RoundtripTime} ms.";
                        return (reply.RoundtripTime > HealthyRountripTime)
                            ? HealthCheckResult.Degraded(msg)
                            : HealthCheckResult.Healthy(msg);

                    default:
                        var err = $"ICMP to {Host} failed: {reply.Status}";
                        return HealthCheckResult.Unhealthy(err);
                }
            }
            catch (Exception ex)
            {
                var err = $"ICMP to {Host} failed: {ex.Message}";
                return HealthCheckResult.Unhealthy(err);
            }
        }
    }
}
