namespace PandaAuth.Shared;

public static class PandeskNativeIdentity
{
    public const string ClientId = "pandesk-native";
    public const string Audience = "oasis-pandesk";
    public const string ReadScope = "oasis.pandesk.customers.read";
    public const string IntrospectionClientId = "oasis-pandesk-introspection";
    public const string LoopbackRegistration = "http://127.0.0.1/oauth/callback";
    public const string CallbackPath = "/oauth/callback";
}
