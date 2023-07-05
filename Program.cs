using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Web;

namespace IssueRepro88390
{
    public class Program
    {
        const bool WANTS_DEADLOCK = true;

        /// <summary>
        /// Minimal example reproducing https://github.com/dotnet/runtime/issues/88390
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();

            TokenCredential azureCredential = new DefaultAzureCredential();

            var keyVaultUri = new Uri("https://example.vault.azure.net/");

            if (WANTS_DEADLOCK)
            {
                // Add Azure Clients using Microsoft.Extensions.Azure
                builder.Services.AddAzureClients(builder =>
                {
                    builder.UseCredential(azureCredential);

                    builder.AddSecretClient(keyVaultUri);
                });
            }
            else
            {
                builder.Services.AddSingleton<SecretClient>(isp => new SecretClient(keyVaultUri, azureCredential));
            }

            builder.Services.ConfigureOptions<ConfigureMicrosoftIdentityOptions>();

            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options => { });

            // This causes the Deadlock
            builder.Services.ConfigureOptions<ConfigureApplicationInsightsServiceOptions>();

            builder.Services.Configure<TelemetryConfiguration>(config =>
            {
                config.SetAzureTokenCredential(azureCredential);
            });

            builder.Services.AddApplicationInsightsTelemetry();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}