using PandaAuth.Shared;
using Xunit;

namespace PandaAuth.Shared.Tests;

public sealed class PandaAuthSessionApiTests
{
    [Fact]
    public void Session_state_routes_and_scope_are_stable_contracts()
    {
        Assert.Equal("panda-session-api", PandaAuthSessionApi.Audience);
        Assert.Equal("session.revocation", PandaAuthSessionApi.Scope);
        Assert.Equal("admin-session-api", PandaAuthSessionApi.AdminClientId);
        Assert.Equal("me-session-api", PandaAuthSessionApi.MeClientId);
        Assert.Equal(TimeSpan.FromHours(2), PandaAuthSessionApi.AdminAbsoluteLifetime);
        Assert.Equal(TimeSpan.FromHours(8), PandaAuthSessionApi.MeAbsoluteLifetime);
        Assert.Equal("/session-state/v1", PandaAuthSessionApi.Prefix);
        Assert.Equal("/session-state/v1/check", PandaAuthSessionApi.Check);
        Assert.Equal("/session-state/v1/revoke", PandaAuthSessionApi.Revoke);
    }

    [Fact]
    public void Session_state_requests_do_not_accept_client_supplied_expiration()
    {
        Assert.Equal(["SessionId"], typeof(SessionStateRevokeRequest)
            .GetProperties().Select(property => property.Name).ToArray());
    }
}
