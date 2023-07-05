using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Azure;

namespace IssueRepro88390
{
    public class Program
    {
        /// <summary>
        /// Minimal example reproducing https://github.com/dotnet/runtime/issues/88390
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();

            TokenCredential azureCredential = new DefaultAzureCredential();

            // Add Azure Clients using Microsoft.Extensions.Azure
            builder.Services.AddAzureClients(builder =>
            {
                builder.UseCredential(azureCredential);

                builder.AddSecretClient(new Uri("https://examplekeyvault.vault.azure.net/"));
            });

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