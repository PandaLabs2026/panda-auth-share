using PandaAuth.Shared;
using Xunit;

namespace PandaAuth.Shared.Tests;

/// <summary>
/// Fleet 控制面身份契约锁线：subject_type 线值与七个控制面 scope 是对外契约，
/// 与元仓 docs/contracts/fleet-lifecycle-v2-proposal.md 一致；漂移即破坏消费方（Fleet 控制面）。
/// </summary>
public sealed class FleetIdentityContractTests
{
    [Fact]
    public void SubjectType_uses_standard_wire_name_and_closed_values()
    {
        Assert.Equal("subject_type", PandaAuthClaims.SubjectType);
        Assert.Equal("human", PandaAuthClaims.SubjectTypes.Human);
        Assert.Equal("machine", PandaAuthClaims.SubjectTypes.Machine);
    }

    [Fact]
    public void Fleet_scopes_are_exactly_the_seven_contract_scopes()
    {
        Assert.Equal(
        [
            "fleet.read",
            "fleet.allocate",
            "fleet.approve",
            "fleet.apply",
            "fleet.retire",
            "fleet.purge",
            "fleet.server.manage",
        ], PandaAuthScopes.FleetAll);
    }

    [Fact]
    public void Fleet_api_is_the_control_plane_audience()
    {
        Assert.Equal("fleet-api", PandaAuthScopes.FleetApi);
    }
}
