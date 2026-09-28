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

/// <summary>租户 ID、产品和规范主机名的一致性映射。</summary>
public static class TenantCanonicalHost
{
    public static string For(TenantId tenantId, TenantProduct product) => product switch
    {
        TenantProduct.PandaAuth => $"{tenantId.Value}.auth.pandalabs.cn",
        TenantProduct.PandaAssistant => $"{tenantId.Value}.assistant.pandalabs.cn",
        TenantProduct.Oasis => $"{tenantId.Value}.oasis.pandalabs.cn",
        _ => throw new ArgumentOutOfRangeException(nameof(product), product, "Unknown tenant product."),
    };
}

/// <summary>由服务端 hostname/Fleet binding 校验后生成的只读租户上下文。</summary>
public sealed record TenantContext
{
    public TenantContext(TenantId tenantId, TenantProduct product, string canonicalHost,
        long routeRevision, TenantRouteState state)
    {
        if (routeRevision < 1) throw new ArgumentOutOfRangeException(nameof(routeRevision));
        var expectedHost = TenantCanonicalHost.For(tenantId, product);
        if (!string.Equals(expectedHost, canonicalHost, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Canonical host does not match tenant and product.", nameof(canonicalHost));

        TenantId = tenantId;
        Product = product;
        CanonicalHost = expectedHost;
        RouteRevision = routeRevision;
        State = state;
    }

    public TenantId TenantId { get; }
    public TenantProduct Product { get; }
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
