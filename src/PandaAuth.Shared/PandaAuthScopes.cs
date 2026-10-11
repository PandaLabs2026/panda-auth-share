namespace PandaAuth.Shared;

/// <summary>
/// Fleet 控制面 scope 与资源常量。契约正本：元仓 <c>docs/contracts/fleet-control-plane-scopes.md</c>（2026-10-11 起；
/// 原 fleet-lifecycle-v2-proposal.md 从未存在，属死指针）。
/// 控制面 scope 与 <see cref="FleetApi"/> 资源绑定共同构成控制面授权面；新增 scope 必须先入契约再登记于此。
/// </summary>
public static class PandaAuthScopes
{
    /// <summary>Fleet 控制面 API 资源：scope 实体的 Resources 绑定目标，即控制面令牌的 audience。</summary>
    public const string FleetApi = "fleet-api";

    /// <summary>只读：控制面状态查询。</summary>
    public const string FleetRead = "fleet.read";

    /// <summary>分配：租户/slot/网络资源分配。</summary>
    public const string FleetAllocate = "fleet.allocate";

    /// <summary>审批：危险操作的人类审批身份（要求 auth_time 距今 ≤ 300s 的已验证重认证）。</summary>
    public const string FleetApprove = "fleet.approve";

    /// <summary>执行：签名计划下发与执行。</summary>
    public const string FleetApply = "fleet.apply";

    /// <summary>退役：租户/产品退役（关入口、拒新登录、排水、停写）。</summary>
    public const string FleetRetire = "fleet.retire";

    /// <summary>清除：仅作用于 closed 资源的独立清除（保留 tombstone/审计/备份）。</summary>
    public const string FleetPurge = "fleet.purge";

    /// <summary>服务器管理：宿主与 agent 生命周期管理。</summary>
    public const string FleetServerManage = "fleet.server.manage";

    /// <summary>
    /// 平台操作：服务目录平台运维操作（health/restart/reload）的发起与轻量审批链路。
    /// fleet-server 的 catalog 端点授权策略与本 scope 字面量耦合（CatalogEndpoints + Program.cs
    /// AddPolicy("fleet.platform.operate")），admin BFF 经 PandaAuth__ActionScopes 申请之；
    /// 2026-10-11 隔离 e2e harness 首跑暴露：此前从未登记，令牌永远不含它，写路径恒 403。
    /// </summary>
    public const string FleetPlatformOperate = "fleet.platform.operate";

    /// <summary>全部 Fleet 控制面 scope，注册（Server）与 seed（DbSeeder）的单一事实源。</summary>
    public static IReadOnlyList<string> FleetAll { get; } =
    [
        FleetRead,
        FleetAllocate,
        FleetApprove,
        FleetApply,
        FleetRetire,
        FleetPurge,
        FleetServerManage,
        FleetPlatformOperate,
    ];
}
