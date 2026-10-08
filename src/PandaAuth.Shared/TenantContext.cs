using System.Text.RegularExpressions;

namespace PandaAuth.Shared;

/// <summary>全局租户标识，固定为 t0000 至 t9999。</summary>
public readonly record struct TenantId
{
    private static readonly Regex Pattern = new("^t[0-9]{4}$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private TenantId(string value) => Value = value;

    public string Value { get; }

    public static TenantId Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Pattern.IsMatch(value))
            throw new FormatException("Tenant ID must match t####.");
        return new TenantId(value);
    }
}

/// <summary>
/// 服务器分区标识（拓扑 v2，ADR-056/061 后）：s000 至 s999。租户入口主机名的分区段。
/// </summary>
public static class TenantZone
{
    private static readonly Regex Pattern = new("^s[0-9]{3}$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static bool IsValid(string? zone) =>
        !string.IsNullOrWhiteSpace(zone) && Pattern.IsMatch(zone);
}

/// <summary>拥有独立租户入口的产品。</summary>
public enum TenantProduct
{
    PandaAuth = 0,
    PandaAssistant = 1,
    Oasis = 2,
}

/// <summary>Fleet 租户入口绑定状态。</summary>
public enum TenantRouteState
{
    Pending = 0,
    Ready = 1,
    Draining = 2,
    Failed = 3,
    Disabled = 4,
}

/// <summary>
/// 租户 ID、产品、分区与规范主机名的一致性映射。
/// 规范形态（拓扑 v2）：t####-&lt;label&gt;.sNNN.pandalabs.cn，label 按产品取 auth/asst/oasis。
/// 历史无分区应用级主机已随 ADR-061 退役，不再是契约形态。
/// </summary>
public static class TenantCanonicalHost
{
    public static string For(TenantId tenantId, TenantProduct product, string zone)
    {
        if (!TenantZone.IsValid(zone))
            throw new ArgumentOutOfRangeException(nameof(zone), zone, "Zone must match sNNN.");
        var label = product switch
        {
            TenantProduct.PandaAuth => "auth",
            TenantProduct.PandaAssistant => "asst",
            TenantProduct.Oasis => "oasis",
            _ => throw new ArgumentOutOfRangeException(nameof(product), product, "Unknown tenant product."),
        };
        return $"{tenantId.Value}-{label}.{zone}.pandalabs.cn";
    }
}

/// <summary>由服务端 hostname/Fleet binding 校验后生成的只读租户上下文。</summary>
public sealed record TenantContext
{
    public TenantContext(TenantId tenantId, TenantProduct product, string zone, string canonicalHost,
        long routeRevision, TenantRouteState state)
    {
        if (routeRevision < 1) throw new ArgumentOutOfRangeException(nameof(routeRevision));
        if (!TenantZone.IsValid(zone)) throw new ArgumentOutOfRangeException(nameof(zone), zone, "Zone must match sNNN (lowercase).");
        var expectedHost = TenantCanonicalHost.For(tenantId, product, zone);
        if (!string.Equals(expectedHost, canonicalHost, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Canonical host does not match tenant, product and zone.", nameof(canonicalHost));

        TenantId = tenantId;
        Product = product;
        Zone = zone;
        CanonicalHost = expectedHost;
        RouteRevision = routeRevision;
        State = state;
    }

    public TenantId TenantId { get; }
    public TenantProduct Product { get; }
    public string Zone { get; }
    public string CanonicalHost { get; }
    public long RouteRevision { get; }
    public TenantRouteState State { get; }
}

public static class TenantContextErrors
{
    public const string UnknownHost = "tenant.unknown_host";
    public const string RouteNotReady = "tenant.route_not_ready";
    public const string ContextMismatch = "tenant.context_mismatch";
}
