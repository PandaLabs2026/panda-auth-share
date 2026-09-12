namespace PandaAuth.Shared;

/// <summary>PandaAuth 对外暴露的 OIDC 端点常量（内部业务应用接入用）。</summary>
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
