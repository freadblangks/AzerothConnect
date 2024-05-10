using AzerothConnect.Configuration;

using Microsoft.Extensions.Options;

using System.Net.Sockets;

namespace AzerothConnect.Services;

public class HealthCheckService : BackgroundService
{
    private readonly ILogger<HealthCheckService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly AzerothConnectOptions options;

    public HealthCheckService(ILogger<HealthCheckService> logger,
        IServiceProvider serviceProvider,
        IOptions<AzerothConnectOptions> options)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(!options.HealthChecksEnabled)
        {
            return;
        }

        logger.LogInformation("Started health check background service.");

        if(!options.Endpoints.ContainsKey("AuthServer"))
        {
            logger.LogError("Failed to find AuthServer endpoint in appsettings.json");
            return;
        }

        var authEndpoint = options.Endpoints["AuthServer"];
        if(string.IsNullOrEmpty(authEndpoint))
        {
            logger.LogError("AuthServer endpoint is null or empty");
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

        while (!stoppingToken.IsCancellationRequested && options.HealthChecksEnabled)
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
