namespace FrontDoorAndCaching
{
    public static class GlobalConstants
    {
        public const string ConfigSectionPrefix = "Config:";

        public const string KeyVaultNameKey = $"{ConfigSectionPrefix}KeyVaultName";
        public const string TenantIdKey = $"{ConfigSectionPrefix}TenantId";
        public const string ManagedIdentityClientIdKey = "ManagedIdentityClientId";
        public const string AzureAppConfigurationConnectionStringKey = "AzureAppConfigurationConnectionString";
        public const string AzureAppConfigurationFilterLabelKey = $"{ConfigSectionPrefix}AzureAppConfigurationFilterLabel";
        public const string AzureAppConfigurationCacheExpirationKey = "AzureAppConfigurationCacheExpiration";
        public const string SentinelKey = "Sentinel";
        public const string ApiEnabled = "ApiEnabled";

    }
}
