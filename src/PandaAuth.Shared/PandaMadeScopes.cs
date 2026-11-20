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

    /// <summary>Platform-level cloud auth issuer (ADR-089 regular value; trailing slash is part of the contract).
    /// ④auth/issuer 原子批(形状 v2 序列④,2026-11-15):正典迁 auth.pandacloud.cc。</summary>
    public const string CloudIssuer = "https://auth.pandacloud.cc/";

    /// <summary>迁移过渡期退役 issuer(旧 token/StaffGrants 存量);F 终局退役批移出白名单。</summary>
    public const string RetiredCloudIssuer = "https://auth.appliket.com/";

    /// <summary>Consumer-side valid issuer allowlist: standalone rollback anchor + cloud auth(+迁移过渡旧值). Fail-closed; extending it requires a share release.</summary>
    public static readonly IReadOnlySet<string> KnownIssuers = new HashSet<string>(StringComparer.Ordinal)
    {
        Issuer,
        CloudIssuer,
    }; // F2 收口(2026-11-16):RetiredCloudIssuer 已移出——旧 token 于 ④原子批全失效,无存量消费方

    public const string AdminClient = "pandamade-admin";
    public const string Api = "pandamade-api";
    public const string Manage = "pandamade.manage";

    // Product accounts use a separate Website client and API audience.
    public const string ProductWebClient = "pandamade-web";
    public const string ProductApi = "pandamade-product-api";
    public const string ProductAccount = "pandamade.account";
    public const string ProductSignInCallback = "https://pandamade.net/auth/signin-oidc";
    public const string ProductSignOutCallback = "https://pandamade.net/auth/signout-callback-oidc";
}
