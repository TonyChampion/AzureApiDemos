using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;
using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace FrontDoorAndCaching.Extensions
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder ConfigureDefaultAppConfiguration(this IHostBuilder builder, string keyVaultNameSetting = GlobalConstants.KeyVaultNameKey, bool configureAzureAppConfigurationRefresh = true)
        {

            return builder.ConfigureAppConfiguration((context, config) =>
            {
                string localSettingsFilename = "appsettings.local.json";
                config.AddJsonFile(localSettingsFilename, optional: true, reloadOnChange: false);

                var builtConfig = config.Build();
                bool reAddLocalSettingsRequired = false;
                string? keyVaultName = builtConfig[keyVaultNameSetting];
                if (!string.IsNullOrWhiteSpace(keyVaultName))
                {
                    var tenantId = builtConfig[GlobalConstants.TenantIdKey];

                    var options = new DefaultAzureCredentialOptions
                    {
                        InteractiveBrowserTenantId = tenantId,
                        SharedTokenCacheTenantId = tenantId,
                        VisualStudioTenantId = tenantId,
                        VisualStudioCodeTenantId = tenantId,
                        ExcludeInteractiveBrowserCredential = true
                    };

                    var managedIdentityClientId = builtConfig[GlobalConstants.ManagedIdentityClientIdKey];
                    if (!string.IsNullOrWhiteSpace(managedIdentityClientId))
                    {
                        options.ManagedIdentityClientId = managedIdentityClientId;
                    }
                    else
                    {
                        options.ExcludeManagedIdentityCredential = true;
                    }

                    config.AddAzureKeyVault(new Uri($"https://{keyVaultName}.vault.azure.net"), new DefaultAzureCredential(options));

                    reAddLocalSettingsRequired = true;
                    builtConfig = config.Build();
                }

                var appConfigurationConnectionString = builtConfig.GetConnectionString(GlobalConstants.AzureAppConfigurationConnectionStringKey);
                if (!string.IsNullOrWhiteSpace(appConfigurationConnectionString))
                {
                    config.AddAzureAppConfiguration(options =>
                    {
                        options.Connect(appConfigurationConnectionString)
                            .Select(KeyFilter.Any)
                            .Select(KeyFilter.Any, builtConfig[GlobalConstants.AzureAppConfigurationFilterLabelKey])
                            .UseFeatureFlags()
                            .ConfigureRefresh(configure =>
                            {
                                configure.Register(GlobalConstants.SentinelKey, refreshAll: true);
                                var cacheExpiration = TimeSpan.FromSeconds(15);
                                configure.SetCacheExpiration(cacheExpiration);
                            });
                    });

                    reAddLocalSettingsRequired = true;
                    builtConfig = config.Build();
                }

                if (reAddLocalSettingsRequired)
                {
                    config.AddJsonFile(localSettingsFilename, optional: true, reloadOnChange: true);
                }
            });
        }

    }
}
