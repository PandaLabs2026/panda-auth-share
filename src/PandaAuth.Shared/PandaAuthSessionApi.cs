namespace PandaAuth.Shared;

/// <summary>
/// Internal session revocation API used by first-party server-side applications.
/// The audience and narrow scope are granted only to dedicated Admin and Me clients.
/// </summary>
public static class PandaAuthSessionApi
{
    public const string Audience = "panda-session-api";
    public const string Scope = "session.revocation";
    public const string AdminClientId = "admin-session-api";
    public const string MeClientId = "me-session-api";
    public static readonly TimeSpan AdminAbsoluteLifetime = TimeSpan.FromHours(2);
    public static readonly TimeSpan MeAbsoluteLifetime = TimeSpan.FromHours(8);
    public const string Prefix = "/session-state/v1";
    public const string Check = Prefix + "/check";
    public const string Revoke = Prefix + "/revoke";
    public const string SsoCheck = Prefix + "/sso/check";
    public const string SsoRevoke = Prefix + "/sso/revoke";
    // Storage partition, never an OAuth client identity. Legacy client partitions remain isolated.
    public const string SsoPartition = "sso-session";
    public static readonly TimeSpan SsoRevocationLifetime = TimeSpan.FromDays(30);
}

public sealed record SessionStateCheckRequest(string SessionId);

public sealed record SessionStateCheckResponse(bool Revoked);

public sealed record SessionStateRevokeRequest(string SessionId);
