using System.Text.RegularExpressions;

namespace PandaAuth.Shared;

/// <summary>全局租户标识，固定为 t0000 至 t9999。</summary>
public readonly record struct TenantId
{
    private static readonly Regex Pattern = new(@"\At[0-9]{4}\z", RegexOptions.CultureInvariant | RegexOptions.Compiled);

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
    private static readonly Regex Pattern = new(@"\As[0-9]{3}\z", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static bool IsValid(string? zone) =>
        !string.IsNullOrWhiteSpace(zone) && Pattern.IsMatch(zone);
}

/// <summary>拥有独立租户入口的产品。</summary>
public enum TenantProduct
{
    PandaAuth = 0,
    PandaAssistant = 1,
    PandaOasis = 2,
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
/// 规范形态（拓扑 v2 / ⑤租户平移 2026-11-15）：t####-&lt;label&gt;.sNNN.pandacloud.cc，label 按产品取 auth/asst/oasis。
/// 迁移窗口内 sNNN.pandalabs.cn 旧后缀仍在解析白名单（F 退役批移出）。
/// 历史无分区应用级主机已随 ADR-061 退役，不再是契约形态。
/// 并存标签（meta ADR-092，2026-11-27）：解析同时接受 -pandy，它是熊猫助理产品 -asst 的并存新写法；
/// 输出（For）在激活批次前仍铸 -asst。
/// </summary>
public static class TenantCanonicalHost
{
    /// <summary>租户 zone 正典后缀（⑤平移后）。</summary>
    public const string ZoneSuffix = "pandacloud.cc";
    /// <summary>迁移窗口退役后缀（解析侧仍在白名单；F 批移出）。</summary>
    public const string RetiredZoneSuffix = "pandalabs.cn";

    /// <summary>
    /// 正典主机名（ADR-056 追记四，2026-11-19）：zone 为空串时铸 **apex 形态**
    /// t####-label.pandacloud.cc（不带分区段——承载机迁移永不换名）；zone 为 sNNN 时铸
    /// 遗留分区形态 t####-label.sNNN.pandacloud.cc（t0000 直连/DR 通道）。
    /// 并存窗口内熊猫助理的输出标签仍为 asst（ADR-092）；解析侧的双收不影响本方法的输出。
    /// </summary>
    public static string For(TenantId tenantId, TenantProduct product, string? zone)
    {
        if (tenantId.Value is null)
            throw new ArgumentException("Tenant ID must be initialized through Parse.", nameof(tenantId));
        var label = product switch
        {
            TenantProduct.PandaAuth => "auth",
            TenantProduct.PandaAssistant => "asst",
            TenantProduct.PandaOasis => "oasis",
            _ => throw new ArgumentOutOfRangeException(nameof(product), product, "Unknown tenant product."),
        };
        return Form(tenantId.Value, label, zone);
    }

    /// <summary>apex 形态便捷方法（等价于 For(tenant, product, zone: null)）。</summary>
    public static string ApexFor(TenantId tenantId, TenantProduct product) => For(tenantId, product, null);

    /// <summary>熊猫助理的并存标签（ADR-092）：TryParse 与 AcceptedHosts 共用这一个取值。</summary>
    private const string AssistantDualLabel = "pandy";

    /// <summary>
    /// 并存窗口内解析会接受的全部规范主机形态（ADR-092）：首项恒为 <see cref="For"/> 的输出，
    /// 其后是并存标签形态——目前只有熊猫助理的 -pandy。供需要逐一比对或登记回调主机的调用方
    /// 使用（回调白名单展开、回调放行策略）；需要单一规范名时仍用 For。
    /// </summary>
    public static IReadOnlyList<string> AcceptedHosts(TenantId tenantId, TenantProduct product, string? zone)
    {
        var canonical = For(tenantId, product, zone);
        return product == TenantProduct.PandaAssistant
            ? [canonical, Form(tenantId.Value, AssistantDualLabel, zone)]
            : [canonical];
    }

    private static string Form(string tenant, string label, string? zone)
    {
        if (string.IsNullOrEmpty(zone))
            return $"{tenant}-{label}.{ZoneSuffix}";
        if (!TenantZone.IsValid(zone))
            throw new ArgumentOutOfRangeException(nameof(zone), zone, "Zone must match sNNN or be empty for the apex form.");
        return $"{tenant}-{label}.{zone}.{ZoneSuffix}";
    }

    /// <summary>租户门户主机（裸 t####，无产品标签）：apex 形态 t####.pandacloud.cc。</summary>
    public static string PortalApexHost(TenantId tenantId) => $"{tenantId.Value}.{ZoneSuffix}";

    /// <summary>
    /// 解析规范租户主机。双形态（ADR-056 追记四）：apex t####-label.pandacloud.cc（三段，
    /// zone 输出为空串）与遗留分区 t####-label.sNNN.pandacloud.cc（四段）。大小写不敏感，
    /// 容忍单个尾点；端口不属于 Host.Host，调用方不得传入 host:port。非规范形态一律 false。
    /// 标签双收（ADR-092）：asst 与 pandy 都解析为 PandaAssistant。
    /// </summary>
    public static bool TryParse(
        string? host, out TenantId tenantId, out TenantProduct product, out string zone)
    {
        tenantId = default;
        product = default;
        zone = string.Empty;
        if (string.IsNullOrWhiteSpace(host)) return false;

        var normalized = host.Trim().ToLowerInvariant();
        if (normalized.EndsWith('.')) normalized = normalized[..^1];
        var labels = normalized.Split('.');
        var isApexForm = labels.Length == 3;
        if (!(isApexForm || labels.Length == 4))
            return false;
        var suffixIndex = labels.Length - 2;
        if (!string.Equals(labels[suffixIndex], "pandacloud", StringComparison.Ordinal) ||
            !string.Equals(labels[suffixIndex + 1], "cc", StringComparison.Ordinal)) // F2 收口:旧后缀已随旧通配删除不可达(2026-11-16)
        {
            return false;
        }

        var head = labels[0];
        var dash = head.IndexOf('-');
        if (dash <= 1 || dash == head.Length - 1) return false;
        TenantId parsedTenant;
        try
        {
            parsedTenant = TenantId.Parse(head[..dash]);
        }
        catch (FormatException)
        {
            return false;
        }

        var parsedZone = isApexForm ? string.Empty : labels[1];
        if (!isApexForm && !TenantZone.IsValid(parsedZone)) return false;

        var parsedProduct = head[(dash + 1)..] switch
        {
            "auth" => TenantProduct.PandaAuth,
            "asst" => TenantProduct.PandaAssistant,
            // 并存标签（meta ADR-092）：-pandy 是熊猫助理 -asst 的并存写法；输出见 For，仍为 asst。
            AssistantDualLabel => TenantProduct.PandaAssistant,
            "oasis" => TenantProduct.PandaOasis,
            _ => (TenantProduct)(-1),
        };
        if (!Enum.IsDefined(parsedProduct)) return false;

        tenantId = parsedTenant;
        product = parsedProduct;
        zone = parsedZone;
        return true;
    }
}

/// <summary>由服务端 hostname/Fleet binding 校验后生成的只读租户上下文。</summary>
public sealed record TenantContext
{
    public TenantContext(TenantId tenantId, TenantProduct product, string? zone, string canonicalHost,
        long routeRevision, TenantRouteState state)
    {
        if (routeRevision < 1) throw new ArgumentOutOfRangeException(nameof(routeRevision));
        if (!string.IsNullOrEmpty(zone) && !TenantZone.IsValid(zone))
            throw new ArgumentOutOfRangeException(nameof(zone), zone, "Zone must match sNNN (lowercase) or be null/empty for the apex form.");
        var expectedHost = TenantCanonicalHost.For(tenantId, product, zone);
        if (!string.Equals(expectedHost, canonicalHost, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Canonical host does not match tenant, product and zone.", nameof(canonicalHost));

        TenantId = tenantId;
        Product = product;
        Zone = zone ?? string.Empty;
        CanonicalHost = expectedHost;
        RouteRevision = routeRevision;
        State = state;
    }

    public TenantId TenantId { get; }
    public TenantProduct Product { get; }

    /// <summary>放置分区；apex 形态租户为空串（<see cref="IsApexForm"/>）。</summary>
    public string Zone { get; }

    /// <summary>apex 形态（无分区段，ADR-056 追记四）：主机名与承载机解耦，迁移永不换名。</summary>
    public bool IsApexForm => Zone.Length == 0;

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
