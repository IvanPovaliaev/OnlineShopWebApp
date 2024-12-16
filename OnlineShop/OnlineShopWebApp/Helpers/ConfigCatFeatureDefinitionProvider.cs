using ConfigCat.Client;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Helpers
{
    public class ConfigCatFeatureDefinitionProvider : IFeatureDefinitionProvider
    {
        private readonly IConfigCatClient _configCatClient;

        public ConfigCatFeatureDefinitionProvider(IOptions<ConfigCatOptions> configCatOptions)
        {
            var key = configCatOptions.Value.Key;
            var pollIntervalInSeconds = configCatOptions.Value.PollIntervalInSeconds;
            _configCatClient = ConfigCatClient.Get(key, options =>
            {
                options.PollingMode = PollingModes.AutoPoll(pollInterval: TimeSpan.FromSeconds(pollIntervalInSeconds));
            });
        }

        public async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
        {
            var randomUser = new User(Guid.NewGuid().ToString());
            var isEnabled = await _configCatClient.GetValueAsync(featureName, false, randomUser);

            return new FeatureDefinition
            {
                Name = featureName,
                EnabledFor = isEnabled
                    ? [new FeatureFilterConfiguration { Name = "AlwaysOn" }]
                    : Enumerable.Empty<FeatureFilterConfiguration>()
            };
        }

        public async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
        {
            var featureKeys = await _configCatClient.GetAllKeysAsync();

            foreach (var key in featureKeys)
            {
                yield return await GetFeatureDefinitionAsync(key);
            }
        }
    }
}
