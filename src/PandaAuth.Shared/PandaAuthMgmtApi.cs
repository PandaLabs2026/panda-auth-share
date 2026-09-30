namespace PandaAuth.Shared;

/// <summary>
/// Management API（M0）公开契约：路由、scope 与资源指示器的单一事实源。
/// 服务端（PandaAuth.Server Features/Management）与消费方统一引用本类，不得复制字面量。
/// </summary>
public static class PandaAuthMgmtApi
{
    /// <summary>资源指示器（audience）：mgmt.* scope 实体绑定该资源，令牌缺此 audience 一律拒绝。</summary>
    public const string Audience = "panda-mgmt-api";

    // scope 契约（M0 冻结面；新增能力以新增 scope 扩展，零破坏）。
    public const string ClientsReadScope = "mgmt.clients.read";
    public const string ClientsWriteScope = "mgmt.clients.write";
    public const string UsersReadScope = "mgmt.users.read";

    // 路由契约（/mgmt/v1 前缀，与 OIDC /connect/* 协议端点显式切割；Auth:Mgmt:Enabled 未启用时整组路由不存在）。
    public const string ClientsRoute = "/mgmt/v1/clients";
    public const string UsersRoute = "/mgmt/v1/users";
}
