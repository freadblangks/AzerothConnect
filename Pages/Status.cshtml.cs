using AzerothConnect.Services;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzerothConnect.Pages;

public class StatusModel : PageModel
{
    public HealthCheckStoreService HealthCheckStoreService { get; }

    public StatusModel(HealthCheckStoreService healthCheckStoreService)
    {
        HealthCheckStoreService = healthCheckStoreService;
    }

    public void OnGet()
    {
    }
}
