namespace PandaAuth.Shared;

/// <summary>
/// Cloud organization configuration v1 identity contract, owned by meta
/// docs/contracts/cloud-configuration-v1/README.md. Registration does not grant
/// target admission or replace the resource server's independent authorization.
/// </summary>
public static class CloudConfigScopes
{
    public const string Issuer = "https://auth.appliket.com/";
    public const string Audience = "panda-cloud-config";
    public const string IntrospectionClient = "cloud-config-api";

    public const string Read = "cloud.config.read";
    public const string ValuesRead = "cloud.config.values.read";
    public const string Create = "cloud.config.create";
    public const string Approve = "cloud.config.approve";
    public const string Execute = "cloud.config.execute";

    public static IReadOnlyList<string> All { get; } = Array.AsReadOnly(new[]
    {
        Read, ValuesRead, Create, Approve, Execute,
    });
}
