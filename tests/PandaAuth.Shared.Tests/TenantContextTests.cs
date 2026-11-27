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
        Assert.Equal("t0042-auth.s001.pandacloud.cc", TenantCanonicalHost.For(tenant, TenantProduct.PandaAuth, "s001"));
        Assert.Equal("t0042-asst.s002.pandacloud.cc", TenantCanonicalHost.For(tenant, TenantProduct.PandaAssistant, "s002"));
        Assert.Equal("t0042-oasis.s001.pandacloud.cc", TenantCanonicalHost.For(tenant, TenantProduct.PandaOasis, "s001"));
    }

    [Fact]
    public void Canonical_host_rejects_uninitialized_tenant_id()
    {
        var error = Assert.Throws<ArgumentException>(() =>
            TenantCanonicalHost.For(default, TenantProduct.PandaAuth, "s001"));
        Assert.Equal("tenantId", error.ParamName);
    }

    [Fact]
    public void Context_rejects_uninitialized_tenant_even_with_matching_empty_host()
    {
        Assert.Throws<ArgumentException>(() => new TenantContext(
            default, TenantProduct.PandaAuth, "s001", "-auth.s001.pandacloud.cc",
            1, TenantRouteState.Ready));
    }

    [Theory]
    [InlineData("t0042\n")]
    [InlineData("t42")]
    [InlineData("t00000")]
    [InlineData("T0042")]
    [InlineData("tenant-0042")]
    public void Rejects_noncanonical_tenant_ids(string value)
    {
        Assert.Throws<FormatException>(() => TenantId.Parse(value));
    }

    [Theory]
    [InlineData("s001\n")]
    [InlineData("s1")]
    [InlineData("s00001")]
    [InlineData("S001")]
    [InlineData("x001")]
    public void Rejects_noncanonical_zones(string zone)
    {
        var tenant = TenantId.Parse("t0042");
        Assert.False(TenantZone.IsValid(zone));
        Assert.Throws<ArgumentOutOfRangeException>(() => TenantCanonicalHost.For(tenant, TenantProduct.PandaAuth, zone));
    }

    [Fact]
    public void Empty_zone_is_the_apex_form_not_an_error()
    {
        // ADR-056 追记四 (2026-11-19): 空串 zone = apex 无分区形态(正典);
        // 旧的「空 zone 必抛」语义废止,仅非空非法值抛。
        var tenant = TenantId.Parse("t0042");
        Assert.Equal("t0042-auth.pandacloud.cc", TenantCanonicalHost.For(tenant, TenantProduct.PandaAuth, ""));
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

    [Fact]
    public void Apex_form_context_has_an_empty_zone_and_apex_canonical_host()
    {
        var ctx = new TenantContext(TenantId.Parse("t0001"), TenantProduct.PandaAuth, null,
            "t0001-auth.pandacloud.cc", 3, TenantRouteState.Ready);

        Assert.True(ctx.IsApexForm);
        Assert.Equal(string.Empty, ctx.Zone);
        Assert.Equal("t0001-auth.pandacloud.cc", ctx.CanonicalHost);
    }

    [Fact]
    public void Zone_form_context_keeps_working()
    {
        var ctx = new TenantContext(TenantId.Parse("t0000"), TenantProduct.PandaAuth, "s001",
            "t0000-auth.s001.pandacloud.cc", 5, TenantRouteState.Ready);

        Assert.False(ctx.IsApexForm);
        Assert.Equal("s001", ctx.Zone);
    }

    [Fact]
    public void Context_for_the_assistant_product_keeps_the_asst_canonical_host()
    {
        // Consumers build the context from For(), not from the request spelling, so a
        // -pandy request resolves to the same context as -asst during the dual-accept window.
        var tenant = TenantId.Parse("t0042");
        Assert.True(TenantCanonicalHost.TryParse("t0042-pandy.s001.pandacloud.cc", out var parsedTenant, out var product, out var zone));
        var ctx = new TenantContext(parsedTenant, product, zone,
            TenantCanonicalHost.For(parsedTenant, product, zone), 1, TenantRouteState.Ready);

        Assert.Equal(tenant, parsedTenant);
        Assert.Equal("t0042-asst.s001.pandacloud.cc", ctx.CanonicalHost);
    }

    [Fact]
    public void Context_rejects_the_dual_label_spelling_as_the_canonical_host()
    {
        // canonicalHost must be the emitted form; the request spelling is not canonical.
        Assert.Throws<ArgumentException>(() => new TenantContext(
            TenantId.Parse("t0042"),
            TenantProduct.PandaAssistant,
            "",
            "t0042-pandy.pandacloud.cc",
            1,
            TenantRouteState.Ready));
    }
}
