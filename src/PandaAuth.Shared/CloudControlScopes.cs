namespace PandaAuth.Shared;

/// <summary>Cloud control-plane capabilities; client admission and target grants are separate.</summary>
public static class CloudControlScopes
{
    public const string Audience = PandaAuthScopes.FleetApi;
    public const string ResourcesRead = "cloud.resources.read";
    public const string ResourcesManage = "cloud.resources.manage";
    public const string SummaryRead = "cloud.resources.summary.read";
    public const string HealthCheck = "cloud.health.check";

    public static IReadOnlyList<string> All { get; } = Array.AsReadOnly(new[]
    {
        ResourcesRead, ResourcesManage, SummaryRead, HealthCheck,
    });
}
