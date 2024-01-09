using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.FeatureManagement;

namespace FrontDoorAndCaching.HealthChecks
{
    public class FeatureEnabledHealthCheck : IHealthCheck
    {
        private readonly IFeatureManager _featureManger;
        private readonly IConfigurationRefresher _refresher;

        public FeatureEnabledHealthCheck(IFeatureManager featureManager, IConfigurationRefresherProvider refresherProvider)
        {
            _featureManger = featureManager;
            _refresher = refresherProvider?.Refreshers?.FirstOrDefault();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            await _refresher.TryRefreshAsync();

            bool isEnabled = await _featureManger.IsEnabledAsync(GlobalConstants.ApiEnabled);
          
            return isEnabled ? new HealthCheckResult(HealthStatus.Healthy) :
                new HealthCheckResult(context.Registration.FailureStatus, "Site is disabled");
        }
    }
}
