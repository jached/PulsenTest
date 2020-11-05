using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using System;

namespace PulsenTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureAppConfiguration((context, builder) =>
                {
                    var config = builder.Build();
                    var keyVaultUrl = config.GetValue<string>("KEY_VAULT_URL");
                    var uaiClientId = config.GetValue<string>("UAI_CLIENT_ID");
                    var isLocalDev = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));

                    AzureServiceTokenProvider azureServiceTokenProvider = isLocalDev
                        ? new AzureServiceTokenProvider()
                        : new AzureServiceTokenProvider($"RunAs=App;AppId={uaiClientId}");

                    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                    config = builder.AddAzureKeyVault(keyVaultUrl, keyVaultClient, new DefaultKeyVaultSecretManager()).Build();
                });
    }
}
