namespace PandaAuth.Shared;

/// <summary>
/// PandaAuth 角色名常量。角色值的权威定义在 server（ASP.NET Core Identity），本类是跨进程契约：
/// 消费方（如 webadmin 的 AdminRole 门禁）必须引用常量而非复制字面量，避免漂移。
/// server 侧 Domain/PandaAuthUser.AdminRole 暂保留自有常量，改引用属后续收口项。
/// </summary>
public static class PandaAuthRoles
{
    /// <summary>管理员角色（roles scope 披露；webadmin 登录门禁依据）。</summary>
    public const string Admin = "admin";
}
