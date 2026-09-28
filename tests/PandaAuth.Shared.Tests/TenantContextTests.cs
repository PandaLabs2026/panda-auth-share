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
        Assert.Equal("t0042.auth.pandalabs.cn", TenantCanonicalHost.For(tenant, TenantProduct.PandaAuth));
        Assert.Equal("t0042.assistant.pandalabs.cn", TenantCanonicalHost.For(tenant, TenantProduct.PandaAssistant));
        Assert.Equal("t0042.oasis.pandalabs.cn", TenantCanonicalHost.For(tenant, TenantProduct.Oasis));
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

    [Fact]
    public void Context_rejects_host_that_does_not_match_tenant_and_product()
    {
        var tenant = TenantId.Parse("t0042");

        Assert.Throws<ArgumentException>(() => new TenantContext(
            tenant,
            TenantProduct.PandaAuth,
            "t0042.assistant.pandalabs.cn",
            1,
            TenantRouteState.Ready));
    }
}
