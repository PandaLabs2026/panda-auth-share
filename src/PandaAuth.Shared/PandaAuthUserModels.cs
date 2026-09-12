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
}
