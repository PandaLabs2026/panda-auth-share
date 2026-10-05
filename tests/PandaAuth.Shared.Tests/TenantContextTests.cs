using PandaAuth.Shared;
using Xunit;

namespace PandaAuth.Shared.Tests;

public sealed class TenantContextTests
{
    [Fact]
    public void Parses_canonical_tenant_id_and_generates_product_host()
    {
        var tenant = TenantId.Parse("t0042");

        Assert.Equal("t0042", tenant.Value);
        Assert.Equal("t0042-auth.s001.pandalabs.cn", TenantCanonicalHost.For(tenant, TenantProduct.PandaAuth, "s001"));
        Assert.Equal("t0042-asst.s002.pandalabs.cn", TenantCanonicalHost.For(tenant, TenantProduct.PandaAssistant, "s002"));
        Assert.Equal("t0042-oasis.s001.pandalabs.cn", TenantCanonicalHost.For(tenant, TenantProduct.Oasis, "s001"));
    }

    [Theory]
    [InlineData("t42")]
    [InlineData("t00000")]
    [InlineData("T0042")]
    [InlineData("tenant-0042")]
    public void Rejects_noncanonical_tenant_ids(string value)
    {
        Assert.Throws<FormatException>(() => TenantId.Parse(value));
    }

    [Theory]
    [InlineData("s1")]
    [InlineData("s00001")]
    [InlineData("S001")]
    [InlineData("x001")]
    [InlineData("")]
    public void Rejects_noncanonical_zones(string zone)
    {
        var tenant = TenantId.Parse("t0042");
        Assert.False(TenantZone.IsValid(zone));
        Assert.Throws<ArgumentOutOfRangeException>(() => TenantCanonicalHost.For(tenant, TenantProduct.PandaAuth, zone));
    }

    [Fact]
    public void Context_rejects_host_that_does_not_match_tenant_product_and_zone()
    {
        var tenant = TenantId.Parse("t0042");

        Assert.Throws<ArgumentException>(() => new TenantContext(
            tenant,
            TenantProduct.PandaAuth,
            "s001",
            "t0042-asst.s001.pandalabs.cn",
            1,
            TenantRouteState.Ready));
        Assert.Throws<ArgumentException>(() => new TenantContext(
            tenant,
            TenantProduct.PandaAuth,
            "s001",
            "t0042-auth.s002.pandalabs.cn",
            1,
            TenantRouteState.Ready));
    }

    [Fact]
    public void Context_rejects_uppercase_zone()
    {
        // zone 是小写规范形：大写直接拒绝（fail-closed），不做归一化。
        Assert.Throws<ArgumentOutOfRangeException>(() => new TenantContext(
            TenantId.Parse("t0042"),
            TenantProduct.PandaAssistant,
            "S001",
            "t0042-asst.S001.pandalabs.cn",
            2,
            TenantRouteState.Ready));
    }
}
