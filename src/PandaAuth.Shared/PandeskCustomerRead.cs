namespace PandaAuth.Shared;

/// <summary>First-party Pandesk access to the Oasis customer read API.</summary>
public static class PandeskCustomerRead
{
    public const string Scope = "oasis.pandesk.customers.read";
    public const string Audience = "oasis-pandesk";
    public const string IntrospectionAudience = "oasis-pandesk-introspection";
    public const string IntrospectionClientId = IntrospectionAudience;
}
