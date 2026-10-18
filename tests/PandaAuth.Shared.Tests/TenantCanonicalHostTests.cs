using PandaAuth.Shared;
using Xunit;

namespace PandaAuth.Shared.Tests;

public sealed class TenantCanonicalHostTests
{
    [Theory]
    [InlineData("t0042-auth.s001.pandalabs.cn", "t0042", TenantProduct.PandaAuth, "s001")]
    [InlineData("T0042-AUTH.S001.PANDALABS.CN", "t0042", TenantProduct.PandaAuth, "s001")]
    [InlineData("t0042-auth.s001.pandalabs.cn.", "t0042", TenantProduct.PandaAuth, "s001")]
    [InlineData("t0042-asst.s002.pandalabs.cn", "t0042", TenantProduct.PandaAssistant, "s002")]
    [InlineData("t0042-oasis.s003.pandalabs.cn", "t0042", TenantProduct.Oasis, "s003")]
    public void Parses_canonical_hosts(
        string host, string tenant, TenantProduct product, string zone)
    {
        Assert.True(TenantCanonicalHost.TryParse(host, out var tenantId, out var parsedProduct, out var parsedZone));
        Assert.Equal(tenant, tenantId.Value);
        Assert.Equal(product, parsedProduct);
        Assert.Equal(zone, parsedZone);
    }

    [Theory]
    [InlineData("")]
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
        Assert.False(TenantCanonicalHost.TryParse(host, out _, out _, out _));
    }
}
