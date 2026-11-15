using PandaAuth.Shared;
using Xunit;

namespace PandaAuth.Shared.Tests;

public sealed class TenantCanonicalHostTests
{
    [Theory]
    [InlineData("t0042-auth.s001.pandacloud.cc", "t0042", TenantProduct.PandaAuth, "s001")]
    [InlineData("t9999-asst.s999.pandacloud.cc", "t9999", TenantProduct.PandaAssistant, "s999")]
    [InlineData("t0000-oasis.s000.pandacloud.cc", "t0000", TenantProduct.PandaOasis, "s000")]
    [InlineData("t0042-auth.s001.pandalabs.cn", "t0042", TenantProduct.PandaAuth, "s001")]
    [InlineData("T0042-AUTH.S001.PANDALABS.CN", "t0042", TenantProduct.PandaAuth, "s001")]
    [InlineData("t0042-auth.s001.pandalabs.cn.", "t0042", TenantProduct.PandaAuth, "s001")]
    [InlineData("t0042-asst.s002.pandalabs.cn", "t0042", TenantProduct.PandaAssistant, "s002")]
    [InlineData("t0042-oasis.s003.pandalabs.cn", "t0042", TenantProduct.PandaOasis, "s003")]
    public void Parses_canonical_hosts(
        string host, string tenant, TenantProduct product, string zone)
    {
        Assert.True(TenantCanonicalHost.TryParse(host, out var tenantId, out var parsedProduct, out var parsedZone));
        Assert.Equal(tenant, tenantId.Value);
        Assert.Equal(product, parsedProduct);
        Assert.Equal(zone, parsedZone);
        Assert.Equal($"{tenant}-{product switch { TenantProduct.PandaAuth => "auth", TenantProduct.PandaAssistant => "asst", _ => "oasis" }}.{zone}.pandacloud.cc", TenantCanonicalHost.For(tenantId, product, parsedZone));
    }

    [Theory]
    [InlineData("")]
    [InlineData("t0042-auth.s001.pandalabs.cn..")]
    [InlineData("t0042-auth.s001.pandacloud.cc.attacker.test")]
    [InlineData("t0042-auth.s001.pandacloud.cn")]
    [InlineData("t0042-auth.s001.pandacloud.cc:443")]
    [InlineData("localhost")]
    [InlineData("auth.pandalabs.cn")]
    [InlineData("t0042.auth.pandalabs.cn")]
    [InlineData("t0042-auth.s01.pandalabs.cn")]
    [InlineData("t0042-auth.s001.pandalabs.com")]
    [InlineData("t42-auth.s001.pandalabs.cn")]
    [InlineData("t0042-web.s001.pandalabs.cn")]
    [InlineData("t0042-auth.s001.pandalabs.cn:443")]
    public void Rejects_non_canonical_hosts(string host)
    {
        Assert.False(TenantCanonicalHost.TryParse(host, out var tenant, out _, out var zone));
        Assert.Equal(default, tenant);
        Assert.Equal(string.Empty, zone);
    }
}
