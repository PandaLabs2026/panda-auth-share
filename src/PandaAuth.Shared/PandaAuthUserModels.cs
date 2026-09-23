namespace PandaAuth.Shared;

/// <summary>用户账号状态。</summary>
public enum UserStatus
{
    Active = 0,
    Frozen = 1,
    Deleted = 2,
}

/// <summary>注册渠道。</summary>
public enum RegisterChannel
{
    Password = 0,
    Sms = 1,
    Email = 2,

    /// <summary>管理后台直接创建（admin 建号端点）。追加值：历史行不受影响。</summary>
    Admin = 3,
}
