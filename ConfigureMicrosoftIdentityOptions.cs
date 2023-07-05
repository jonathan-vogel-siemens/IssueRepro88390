using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueRepro88390
{
    public class ConfigureMicrosoftIdentityOptions : IConfigureNamedOptions<MicrosoftIdentityOptions>
    {
        /// <summary>
        /// Simply consume the SecretClient - Works just fine
        /// </summary>
        /// <param name="secretClient">the SecretClient</param>
        public ConfigureMicrosoftIdentityOptions(
            SecretClient secretClient)
        {
            SecretClient = secretClient;
        }

        public SecretClient SecretClient { get; }

        public void Configure(MicrosoftIdentityOptions options)
        {
            KeyVaultSecret secret = SecretClient.GetSecret("secretAppSecret");
            options.ClientSecret = secret.Value;

            options.ResponseType = "id_token";
        }

        public void Configure(string name, MicrosoftIdentityOptions options) => Configure(options);
    }
}
