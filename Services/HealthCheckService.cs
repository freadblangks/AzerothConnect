
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Security.Cryptography.Xml;

namespace AzerothConnect.Services
{
    public class HealthCheckService : BackgroundService
    {
        private readonly ILogger<HealthCheckService> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration config;

        public HealthCheckService(ILogger<HealthCheckService> logger,
            IServiceProvider serviceProvider,
            IConfiguration config)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Started health check background service.");

            var authEndpoint = config.GetSection("Endpoints:AzerothCore:Auth").Value;
            if(string.IsNullOrEmpty(authEndpoint))
            {
                logger.LogError("Failed to get auth endpoint. Ensure endpoints are configured in appsettings.json.");
                return;
            }

            int port = 3724;
            string[] authSections = authEndpoint.Split(":");
            string host = authSections[0];

            if(authSections.Length > 1)
            {
                if(!int.TryParse(authSections[1], out port))
                {
                    logger.LogError("Invalid port found for auth endpoint.");
                }
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = serviceProvider.CreateAsyncScope())
                {
                    var now = DateTime.Now;
                    bool result = false;
                    var client = new TcpClient();

                    logger.LogInformation("Fetching auth server status..");

                    try
                    {
                        await client.ConnectAsync(host, port);
                        result = true;
                    }
                    catch(Exception ex)
                    {
                        logger.LogError($"An error occured while trying to fetch auth server status: {ex.Message}");
                        result = false;
                    }
                    finally
                    {
                        client.Close();
                    }

                    var healthStore = scope.ServiceProvider.GetRequiredService<HealthCheckStoreService>();
                    healthStore.AuthStatus = result;

                    logger.LogInformation($"Finished fetching auth server status in {(DateTime.Now - now).TotalMilliseconds}ms.");

                    await Task.Delay(10000);
                }
            }
        }
    }
}
