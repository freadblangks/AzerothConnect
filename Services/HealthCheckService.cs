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

        var worldEndpoint = options.Endpoints["WorldServer"];
        if (string.IsNullOrEmpty(worldEndpoint))
        {
            logger.LogError("WorldServer endpoint is null or empty");
            return;
        }

        int authPort = 3724;
        string[] authSections = authEndpoint.Split(":");
        string authHost = authSections[0];

        if(authSections.Length > 1)
        {
            if(!int.TryParse(authSections[1], out authPort))
            {
                logger.LogError("Invalid port found for auth endpoint.");
            }
        }

        int worldPort = 8085;
        string[] worldSections = worldEndpoint.Split(":");
        string worldHost = worldSections[0];

        if (worldSections.Length > 1)
        {
            if (!int.TryParse(worldSections[1], out worldPort))
            {
                logger.LogError("Invalid port found for world endpoint.");
            }
        }


        while (!stoppingToken.IsCancellationRequested && options.HealthChecksEnabled)
        {
            using (IServiceScope scope = serviceProvider.CreateAsyncScope())
            {
                var now = DateTime.Now;

                logger.LogInformation("Fetching auth server status..");

                var healthStore = scope.ServiceProvider.GetRequiredService<HealthCheckStoreService>();

                healthStore.AuthStatus = await TestConnection(authHost, authPort);
                healthStore.WorldStatus = await TestConnection(worldHost, worldPort);

                logger.LogInformation($"Finished fetching auth server status in {(DateTime.Now - now).TotalMilliseconds}ms.");

                await Task.Delay(10000);
            }
        }
    }

    private async Task<bool> TestConnection(string host, int port)
    {
        var client = new TcpClient();
        bool result = false;

        logger.LogInformation($"Testing connection for host '{host}' port '{port}'.");

        try
        {
            await client.ConnectAsync(host, port);
            result = true;
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occured while trying to fetch auth server status: {ex.Message}");
            result = false;
        }
        finally
        {
            client.Close();
        }

        return result;
    }
}
