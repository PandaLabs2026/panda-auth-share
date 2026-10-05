using PandaAuth.Shared;
using Xunit;

namespace PandaAuth.Shared.Tests;

/// <summary>
/// 管理 API 契约的路径段转义锁线：路由参数一律 Uri.EscapeDataString——参数来自 admin 侧
/// 路由解码，可含 ?、%、. 等保留字符，裸插值经 HttpClient 的 Uri 规范化可注入 query
/// 或干扰路径段。锁两层：保留字符必转义（? 与 .. 不得在上游路径被重新解释），
/// 合法 id 逐字不变（契约对既有调用方幂等）。
/// </summary>
public sealed class PandaAuthAdminApiPathTests
{
    [Theory]
    [InlineData("u/1?x=1", "/admin-api/users/u%2F1%3Fx%3D1")]
    [InlineData("a/../b", "/admin-api/users/a%2F..%2Fb")]
    [InlineData("百分%比", "/admin-api/users/%E7%99%BE%E5%88%86%25%E6%AF%94")]
    [InlineData("u-123", "/admin-api/users/u-123")]
    public void User_escapes_reserved_characters_and_keeps_plain_ids(string id, string expected)
    {
        Assert.Equal(expected, PandaAuthAdminApi.User(id));
    }

    [Fact]
    public void Reserved_characters_never_leak_into_query_or_extra_segments()
    {
        var adversarial = "u?redirect=/evil&x=1/../../admin";

        // UserClaims 是多层拼接代表：转义后整个 id 仍是**单一路径段**——? 不拆 query、
        // / 不增段、.. 无法导航；这正是裸插值下 HttpClient Uri 规范化会放大的注入面。
        Assert.Equal(
            "/admin-api/claims/users/u%3Fredirect%3D%2Fevil%26x%3D1%2F..%2F..%2Fadmin",
            PandaAuthAdminApi.UserClaims(adversarial));
    }

    [Theory]
    [InlineData("t0000.web")]
    [InlineData("u-123")]
    public void Plain_ids_are_unchanged_across_contract_methods(string id)
    {
        // 合法 id（无保留字符）转义为幂等：既有调用方（server 侧 Created Location、
        // admin BFF 转发、SDK）行为不变——这是「纯转义、无回退」的锁线。
        Assert.Equal($"/admin-api/users/{id}", PandaAuthAdminApi.User(id));
        Assert.Equal($"/admin-api/users/{id}/status", PandaAuthAdminApi.UserStatus(id));
        Assert.Equal($"/admin-api/users/{id}/reset-password", PandaAuthAdminApi.UserResetPassword(id));
        Assert.Equal($"/admin-api/users/{id}/roles", PandaAuthAdminApi.UserRoles(id));
        Assert.Equal($"/admin-api/users/{id}/unlock", PandaAuthAdminApi.UserUnlock(id));
        Assert.Equal($"/admin-api/users/{id}/profile", PandaAuthAdminApi.UserProfile(id));
        Assert.Equal($"/admin-api/users/{id}/reset-2fa", PandaAuthAdminApi.UserResetTwoFactor(id));
        Assert.Equal($"/admin-api/users/{id}/deactivate", PandaAuthAdminApi.UserDeactivate(id));
        Assert.Equal($"/admin-api/claims/users/{id}", PandaAuthAdminApi.UserClaims(id));
        Assert.Equal($"/admin-api/claims/roles/{id}", PandaAuthAdminApi.RoleClaims(id));
        Assert.Equal($"/admin-api/clients/{id}", PandaAuthAdminApi.Client(id));
        Assert.Equal($"/admin-api/clients/{id}/redirect-uris", PandaAuthAdminApi.ClientRedirectUris(id));
        Assert.Equal($"/admin-api/clients/{id}/permissions", PandaAuthAdminApi.ClientPermissions(id));
        Assert.Equal($"/admin-api/clients/{id}/rotate-secret", PandaAuthAdminApi.ClientRotateSecret(id));
    }

    [Fact]
    public void Composite_paths_keep_numeric_claim_ids_unescaped()
    {
        // claimId 是 long 路由约束值，无保留字符可能，不转义；string 参数照常转义。
        Assert.Equal(
            "/admin-api/claims/users/u%2F1/9",
            PandaAuthAdminApi.UserClaim("u/1", 9));
        Assert.Equal(
            "/admin-api/claims/roles/r%2F1/9",
            PandaAuthAdminApi.RoleClaim("r/1", 9));
    }
}
