namespace PandaAuth.Shared;

/// <summary>PandaAuth 对外暴露的 OIDC 端点常量（内部业务应用接入用）。</summary>
/// <remarks>
/// 以下取值为单一事实源：服务端以本类常量注册端点，消费方（Server、SDK、Me）同样引用本类，
/// 不得在各自仓内复制字面量。
/// <para>
/// 核验记录（2026-09-16）：8 个常量已逐条对生产 discovery
/// （https://auth.pandalabs.cc/.well-known/openid-configuration）核验，8/8 全部一致。
/// </para>
/// <para>
/// 注意 <see cref="JsonWebKeySet"/>：它对应 discovery 文档的 <c>jwks_uri</c>，即 OpenIddict 的 JWKS 路由
/// <c>/.well-known/jwks</c>。实测该路径返回 200（528 字节，真 JWKS 内容），而 <c>/connect/jwks</c> 返回 404，
/// 因此<b>不要把它误改为 <c>/connect/jwks</c></b>。
/// </para>
/// </remarks>
public static class PandaAuthEndpoints
{
    public const string Authorization = "/connect/authorize";
    public const string Token = "/connect/token";
    public const string Userinfo = "/connect/userinfo";
    public const string Logout = "/connect/logout";
    public const string Introspection = "/connect/introspect";
    public const string Revocation = "/connect/revoke";
    public const string JsonWebKeySet = "/.well-known/jwks";
    public const string OpenIdConfiguration = "/.well-known/openid-configuration";
}
