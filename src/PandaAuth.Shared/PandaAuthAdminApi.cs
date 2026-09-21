namespace PandaAuth.Shared;

/// <summary>
/// 管理后台数据 API 的端点契约：webadmin BFF ↔ IDP 的**内部通道**（BFF 直连 IDP
/// `Auth:IdpInternalBaseAddress`，不经公网与 Caddy）。鉴权为 Bearer Access Token
/// （OpenIddict Server 方案）+ admin 角色；调用方（webadmin）与实现方（server）共用本常量组，
/// 避免 URL 字面量两端漂移。路径刻意避开公网路由前缀（/connect、/account、/admin）。
/// </summary>
public static class PandaAuthAdminApi
{
    public const string Prefix = "/admin-api";

    // ---- 用户管理 ----
    public const string Users = Prefix + "/users";

    public static string User(string id) => $"{Users}/{id}";

    public static string UserStatus(string id) => $"{Users}/{id}/status";

    public static string UserResetPassword(string id) => $"{Users}/{id}/reset-password";

    public static string UserRoles(string id) => $"{Users}/{id}/roles";

    public static string UserUnlock(string id) => $"{Users}/{id}/unlock";

    public static string UserProfile(string id) => $"{Users}/{id}/profile";

    public static string UserResetTwoFactor(string id) => $"{Users}/{id}/reset-2fa";

    public static string UserDeactivate(string id) => $"{Users}/{id}/deactivate";

    // ---- Claims 管理 ----
    public const string Claims = Prefix + "/claims";

    public static string UserClaims(string userId) => $"{Claims}/users/{userId}";

    public static string UserClaim(string userId, long claimId) => $"{UserClaims(userId)}/{claimId}";

    public static string RoleClaims(string roleId) => $"{Claims}/roles/{roleId}";

    public static string RoleClaim(string roleId, long claimId) => $"{RoleClaims(roleId)}/{claimId}";

    // ---- 客户端管理 ----
    public const string Clients = Prefix + "/clients";

    public const string ClientOptions = Clients + "/options";

    public static string Client(string clientId) => $"{Clients}/{clientId}";

    public static string ClientRedirectUris(string clientId) => $"{Clients}/{clientId}/redirect-uris";

    public static string ClientPermissions(string clientId) => $"{Clients}/{clientId}/permissions";

    public static string ClientRotateSecret(string clientId) => $"{Clients}/{clientId}/rotate-secret";

    // ---- 审计查询（只读） ----
    public const string AuditLogins = Prefix + "/audit/logins";

    public const string AuditAdmin = Prefix + "/audit/admin";
}

/// <summary>
/// 管理操作审计动作名。只审计**变更**（冻结/重置/改白名单/改权限/轮换密钥），不审计读——
/// 读操作的量级会把审计表变成访问日志。命名「域.动作」，新增动作时同步 webadmin 展示映射。
/// </summary>
public static class AdminAuditAction
{
    public const string UserCreate = "user.create";

    public const string UserFreeze = "user.freeze";

    public const string UserUnfreeze = "user.unfreeze";

    public const string UserResetPassword = "user.reset_password";

    public const string UserUpdateRoles = "user.update_roles";

    public const string UserUnlock = "user.unlock";

    public const string UserUpdateProfile = "user.update_profile";

    public const string UserResetTwoFactor = "user.reset_2fa";

    public const string UserDeactivate = "user.deactivate";

    public const string ClientUpdateUris = "client.update_uris";

    public const string ClientUpdatePermissions = "client.update_permissions";

    public const string ClientRotateSecret = "client.rotate_secret";

    public const string UserAddClaim = "user.add_claim";

    public const string UserRemoveClaim = "user.remove_claim";

    public const string RoleAddClaim = "role.add_claim";

    public const string RoleRemoveClaim = "role.remove_claim";
}

// ---------- DTO（server 序列化、webadmin 反序列化共用；字段即契约） ----------
// 序列化走 ASP.NET Core 默认 camelCase JSON；时间统一 DateTimeOffset（ISO 8601）。

public sealed record AdminPageResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);

public sealed record AdminUserSummary(
    string Id,
    string UserName,
    string? Email,
    string? Nickname,
    UserStatus Status,
    DateTimeOffset CreatedAt);

public sealed record AdminUserDetail(
    string Id,
    string UserName,
    string? Email,
    bool EmailConfirmed,
    string? Nickname,
    UserStatus Status,
    IReadOnlyList<string> Roles,
    DateTimeOffset? LockoutEnd,
    int AccessFailedCount,
    bool TwoFactorEnabled,
    RegisterChannel RegisterChannel,
    string? Region,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

/// <summary>冻结 / 解冻请求。只允许 Active 与 Frozen 互转（Deleted 不经此端点）。 </summary>
public sealed record AdminUserStatusRequest(UserStatus Status);

/// <summary>重置密码请求；NewPassword 缺省时由服务端生成合规随机密码（明文仅返回一次）。 </summary>
public sealed record AdminResetPasswordRequest(string? NewPassword);

public sealed record AdminResetPasswordResponse(string Password);

/// <summary>
/// 管理端建号请求。Password 空/缺省时服务端生成合规随机密码；GrantAdminRole 创建后立即授予 admin 角色。
/// </summary>
public sealed record AdminCreateUserRequest(
    string UserName,
    string? Email,
    string? Nickname,
    string? Region,
    string? Password,
    bool GrantAdminRole);

/// <summary>建号响应；Password 仅在服务端生成时返回一次，管理员指定密码时不回传明文。</summary>
public sealed record AdminCreateUserResponse(string Id, string UserName, string? Email, string? Password);

/// <summary>角色变更请求：全量替换语义——Roles 即目标用户的完整角色集合。</summary>
public sealed record AdminUserRolesRequest(IReadOnlyList<string> Roles);

/// <summary>资料编辑请求：PUT 全量语义——每个字段都携带最终值，null 即清空。</summary>
public sealed record AdminUserProfileRequest(string? Email, string? Nickname, string? Region);

/// <summary>注销请求：ConfirmUserName 必须与目标用户名逐字相等（防误触主门禁）。注销为终态。</summary>
public sealed record AdminDeactivateRequest(string ConfirmUserName);

/// <summary>自定义 Claims 请求：仅允许服务端校验通过的 panda:* 命名空间和明确 scope。</summary>
public sealed record AdminClaimRequest(string ClaimType, string ClaimValue, string Scope);

public sealed record AdminUserClaimEntry(long Id, string UserId, string ClaimType, string ClaimValue, string Scope);

public sealed record AdminRoleClaimEntry(long Id, string RoleId, string ClaimType, string ClaimValue, string Scope);

public sealed record AdminClientSummary(
    string ClientId,
    string? DisplayName,
    string ClientType,
    string ConsentType);

public sealed record AdminClientDetail(
    string ClientId,
    string? DisplayName,
    string ClientType,
    string ConsentType,
    IReadOnlyList<string> RedirectUris,
    IReadOnlyList<string> PostLogoutRedirectUris,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<string> Requirements);

public sealed record AdminRedirectUrisRequest(
    IReadOnlyList<string> RedirectUris,
    IReadOnlyList<string> PostLogoutRedirectUris);

public sealed record AdminPermissionsRequest(IReadOnlyList<string> Permissions);

public sealed record AdminRotateSecretResponse(string ClientId, string ClientSecret);

public sealed record AdminLoginLogEntry(
    long Id,
    string? UserId,
    string UserName,
    string? ClientId,
    string? IpAddress,
    string? UserAgent,
    bool Succeeded,
    string? FailureReason,
    DateTimeOffset CreatedAt);

public sealed record AdminAuditLogEntry(
    long Id,
    string ActorUserId,
    string? ActorUserName,
    string Action,
    string? TargetType,
    string? TargetId,
    string? Detail,
    string? IpAddress,
    DateTimeOffset CreatedAt);

/// <summary>权限目录：服务端 allowlist 的单一事实源，UI 复选组由它驱动（group 为展示分组）。 </summary>
public sealed record AdminClientOptions(IReadOnlyList<AdminOptionGroup> PermissionGroups);

public sealed record AdminOptionGroup(string Group, IReadOnlyList<string> Options);
