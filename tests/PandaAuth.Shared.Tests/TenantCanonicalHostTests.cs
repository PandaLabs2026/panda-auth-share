using PandaAuth.Shared;
using Xunit;

namespace PandaAuth.Shared.Tests;

public sealed class TenantCanonicalHostTests
{
    [Theory]
    [InlineData("t0042-auth.s001.pandacloud.cc", "t0042", TenantProduct.PandaAuth, "s001")]
    [InlineData("t9999-asst.s999.pandacloud.cc", "t9999", TenantProduct.PandaAssistant, "s999")]
    [InlineData("t0000-oasis.s000.pandacloud.cc", "t0000", TenantProduct.PandaOasis, "s000")]
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
    [InlineData("t0042-auth.s001.pandalabs.cn")] // F2 收口:旧后缀已随旧通配 DNS 删除不可达
    [InlineData("t0042-auth.s001.pandalabs.cn..")]
    [InlineData("t0042-auth.s001.pandacloud.cc.attacker.test")]
    [InlineData("t0042-auth.s001.pandacloud.cn")]
    [InlineData("t0042-auth.s001.pandacloud.cc:443")]
    [InlineData("t0042\n-auth.s001.pandacloud.cc")]
    [InlineData("t0042-auth.s001\n.pandacloud.cc")]
    [InlineData("t0042\n-auth.s001.pandalabs.cn")]
    [InlineData("t0042-auth.s001\n.pandalabs.cn")]
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

    [Theory]
    [InlineData("t0001-auth.pandacloud.cc", "t0001", TenantProduct.PandaAuth)]
    [InlineData("t0001-asst.pandacloud.cc", "t0001", TenantProduct.PandaAssistant)]
    [InlineData("t0001-oasis.pandacloud.cc", "t0001", TenantProduct.PandaOasis)]
    public void Parses_apex_hosts_without_a_zone_segment(string host, string tenant, TenantProduct product)
    {
        // ADR-056 追记四 (2026-11-19): apex canonical hosts carry no zone
        // segment; the zone output is the empty string.
        Assert.True(TenantCanonicalHost.TryParse(host, out var tenantId, out var parsedProduct, out var parsedZone));
        Assert.Equal(tenant, tenantId.Value);
        Assert.Equal(product, parsedProduct);
        Assert.Equal(string.Empty, parsedZone);
        Assert.Equal(host, TenantCanonicalHost.ApexFor(tenantId, parsedProduct));
    }

    [Fact]
    public void For_builds_both_forms_and_the_apex_portal_host()
    {
        var tenant = TenantId.Parse("t0001");

        Assert.Equal("t0001-auth.pandacloud.cc", TenantCanonicalHost.For(tenant, TenantProduct.PandaAuth, null));
        Assert.Equal("t0001-auth.pandacloud.cc", TenantCanonicalHost.ApexFor(tenant, TenantProduct.PandaAuth));
        Assert.Equal("t0001-asst.s002.pandacloud.cc", TenantCanonicalHost.For(tenant, TenantProduct.PandaAssistant, "s002"));
        Assert.Equal("t0001.pandacloud.cc", TenantCanonicalHost.PortalApexHost(tenant));
    }

    [Theory]
    [InlineData("t0042-auth.pandacloud.cc.attacker.test")]
    [InlineData("t0042-auth.pandacloud.cn")]
    [InlineData("t0042-auth.pandacloud.cc:443")]
    [InlineData("t0042.pandacloud.cc")]        // 裸门户主机不是产品主机,不产出产品上下文
    [InlineData("t0042-auth..pandacloud.cc")]
    [InlineData("t0042-auth.s001.pandacloud.cc")] // 分区形态仍合法(不属负例,断言由上组覆盖)
    public void Apex_parsing_still_fails_non_canonical_shapes(string host)
    {
        if (host == "t0042-auth.s001.pandacloud.cc")
        {
            Assert.True(TenantCanonicalHost.TryParse(host, out _, out _, out var zone));
            Assert.Equal("s001", zone);
            return;
        }
        Assert.False(TenantCanonicalHost.TryParse(host, out _, out _, out _));
    }

    [Theory]
    [InlineData("t0042-pandy.pandacloud.cc", "t0042", "")]
    [InlineData("t0042-pandy.pandacloud.cc.", "t0042", "")]
    [InlineData("t0042-pandy.s001.pandacloud.cc", "t0042", "s001")]
    [InlineData("T0042-PANDY.S001.PandaCloud.CC", "t0042", "s001")]
    public void Parses_the_dual_pandy_label_as_the_assistant_product(string host, string tenant, string zone)
    {
        // ADR-092 dual-accept window: -pandy is the registered spelling of the
        // Assistant product next to -asst. Parsing accepts both; For() still emits -asst.
        Assert.True(TenantCanonicalHost.TryParse(host, out var tenantId, out var product, out var parsedZone));
        Assert.Equal(tenant, tenantId.Value);
        Assert.Equal(TenantProduct.PandaAssistant, product);
        Assert.Equal(zone, parsedZone);
    }

    [Fact]
    public void For_still_emits_the_asst_label_during_the_dual_accept_window()
    {
        var tenant = TenantId.Parse("t0042");

        Assert.Equal("t0042-asst.pandacloud.cc", TenantCanonicalHost.ApexFor(tenant, TenantProduct.PandaAssistant));
        Assert.Equal("t0042-asst.s001.pandacloud.cc", TenantCanonicalHost.For(tenant, TenantProduct.PandaAssistant, "s001"));
    }

    [Theory]
    [InlineData("t0042-pandyy.pandacloud.cc")]
    [InlineData("t0042-pandy-.pandacloud.cc")]
    [InlineData("t0042--pandy.pandacloud.cc")]
    [InlineData("t0042-pandy.s01.pandacloud.cc")]
    [InlineData("t0042-pandy.pandacloud.cn")]
    [InlineData("t0042-pandy.s001.pandacloud.cc.attacker.test")]
    public void Rejects_pandy_lookalikes(string host)
    {
        Assert.False(TenantCanonicalHost.TryParse(host, out _, out _, out _));
    }

    [Theory]
    [InlineData(TenantProduct.PandaAuth, null, "t0042-auth.pandacloud.cc")]
    [InlineData(TenantProduct.PandaOasis, "s003", "t0042-oasis.s003.pandacloud.cc")]
    public void AcceptedHosts_lists_only_the_canonical_form_for_products_without_a_dual_label(
        TenantProduct product, string? zone, string canonical)
    {
        var hosts = TenantCanonicalHost.AcceptedHosts(TenantId.Parse("t0042"), product, zone);

        Assert.Equal(new[] { canonical }, hosts);
    }

    [Theory]
    [InlineData(null, "t0042-asst.pandacloud.cc", "t0042-pandy.pandacloud.cc")]
    [InlineData("s003", "t0042-asst.s003.pandacloud.cc", "t0042-pandy.s003.pandacloud.cc")]
    public void AcceptedHosts_puts_the_canonical_form_first_and_the_dual_label_form_after_it(
        string? zone, string canonical, string dual)
    {
        var hosts = TenantCanonicalHost.AcceptedHosts(TenantId.Parse("t0042"), TenantProduct.PandaAssistant, zone);

        Assert.Equal(new[] { canonical, dual }, hosts);
    }

    [Fact]
    public void Every_accepted_host_parses_back_to_the_same_tenant_product_and_zone()
    {
        foreach (var product in Enum.GetValues<TenantProduct>())
        {
            foreach (var zone in new string?[] { null, string.Empty, "s001" })
            {
                var tenant = TenantId.Parse("t1234");
                foreach (var host in TenantCanonicalHost.AcceptedHosts(tenant, product, zone))
                {
                    Assert.True(TenantCanonicalHost.TryParse(host, out var parsedTenant, out var parsedProduct, out var parsedZone));
                    Assert.Equal(tenant, parsedTenant);
                    Assert.Equal(product, parsedProduct);
                    Assert.Equal(zone ?? string.Empty, parsedZone);
                }
            }
        }
    }

    [Fact]
    public void AcceptedHosts_validates_the_zone_like_For()
    {
        var tenant = TenantId.Parse("t0042");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => TenantCanonicalHost.AcceptedHosts(tenant, TenantProduct.PandaAssistant, "s01"));
    }
}
