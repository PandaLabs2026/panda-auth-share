namespace PandaAuth.Shared;

/// <summary>PandaAuth 自定义 Claim 类型常量（标准 OIDC Claim 用 OpenIddictConstants）。</summary>
public static class PandaAuthClaims
{
    /// <summary>用户昵称（profile scope）。</summary>
    public const string Nickname = "nickname";

    /// <summary>由服务端校验后写入的租户 ID；浏览器不得自行提交或覆盖。</summary>
    public const string TenantId = "panda:tenant_id";

    /// <summary>由服务端校验后写入的租户规范主机名。</summary>
    public const string TenantHost = "panda:tenant_host";
}
