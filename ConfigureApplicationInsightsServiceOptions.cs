using Azure.Security.KeyVault.Secrets;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueRepro88390
{
    public class ConfigureApplicationInsightsServiceOptions : IConfigureOptions<ApplicationInsightsServiceOptions>
    {
        /// <summary>
        /// Simply consume the SecretClient - Deadlock
        /// </summary>
        /// <param name="secretClient">the SecretClient</param>
        public ConfigureApplicationInsightsServiceOptions(
            SecretClient secretClient)
        {
            SecretClient = secretClient;
        }

        public SecretClient SecretClient { get; }

        public void Configure(ApplicationInsightsServiceOptions options)
        {
            // KeyVaultSecret secret = SecretClient.GetSecret($"appinsights-connectionstring");
            // options.ConnectionString = secret.Value;
        }

        public void Configure(string name, ApplicationInsightsServiceOptions options) => Configure(options);
    }
}
