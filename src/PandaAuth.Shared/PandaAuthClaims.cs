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

    /// <summary>
    /// OIDC 标准 subject_type（Fleet 控制面 introspection 契约）：区分人类主体与机器主体。
    /// 值域由 <see cref="SubjectTypes"/> 封闭；机器主体永不充当人类审批身份。
    /// </summary>
    public const string SubjectType = "subject_type";

    /// <summary><see cref="SubjectType"/> 的受控取值。</summary>
    public static class SubjectTypes
    {
        /// <summary>IDP 校验过的真实登录会话（auth_time 为原始登录时刻，refresh/MFA 标记不得前移）。</summary>
        public const string Human = "human";

        /// <summary>client_credentials 服务间主体，无终端用户语义。</summary>
        public const string Machine = "machine";
    }
}
