namespace PandaAuth.Shared;

/// <summary>PandaMade independent identity pool contract; owner: PandaAuth coordinator docs/contracts/pandamade-identity.md.</summary>
public static class PandaMadeScopes
{
    public const string Issuer = "https://auth.pandamade.net/";
    public const string AdminClient = "pandamade-admin";
    public const string Api = "pandamade-api";
    public const string Manage = "pandamade.manage";
}
