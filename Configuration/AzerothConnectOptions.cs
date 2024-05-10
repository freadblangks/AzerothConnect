namespace AzerothConnect.Configuration;

public class AzerothConnectOptions
{
    public const string SectionName = "AzerothConnect";

    public bool HealthChecksEnabled { get; set; }
    public Dictionary<string, string> Endpoints { get; set; } = new Dictionary<string, string>();

    public AzerothConnectOptions()
    {
        HealthChecksEnabled = true;
    }
}
