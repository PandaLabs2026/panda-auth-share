namespace PandaAuth.Shared;

/// <summary>
/// PandaMade identity contract; owner: PandaAuth coordinator docs/contracts/pandamade-identity.md.
/// ADR-089 (2026-10-23): PandaMade system-level authentication consolidates into the platform-level
/// cloud auth (auth.appliket.com). Issuer becomes a deploy-time choice within a fixed allowlist;
/// client ids and scope names are unchanged.
/// </summary>
public static class PandaMadeScopes
{
    /// <summary>
    /// Standalone instance issuer (auth.pandamade.net, tcloud-sg-01). Retained as the ADR-089
    /// rollback anchor: the legacy registration path (panda-auth-server --register-pandamade-clients
    /// and its production guard) still pins this value, and consumer validation keeps accepting it.
    /// </summary>
    public const string Issuer = "https://auth.pandamade.net/";

    /// <summary>Platform-level cloud auth issuer (ADR-089 regular value; trailing slash is part of the contract).</summary>
    public const string CloudIssuer = "https://auth.appliket.com/";

    /// <summary>Consumer-side valid issuer allowlist: standalone rollback anchor + cloud auth. Fail-closed; extending it requires a share release.</summary>
    public static readonly IReadOnlySet<string> KnownIssuers = new HashSet<string>(StringComparer.Ordinal)
    {
        Issuer,
        CloudIssuer,
    };

    public const string AdminClient = "pandamade-admin";
    public const string Api = "pandamade-api";
    public const string Manage = "pandamade.manage";
}
