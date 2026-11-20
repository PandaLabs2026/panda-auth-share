using System.Reflection;
using PandaAuth.Shared;
using Xunit;

public class PandaMadeContractTests
{
    [Fact]
    public void PandaMadeIdentityIsSeparateFromFleet()
    {
        var contract = typeof(PandaAuthScopes).Assembly.GetType("PandaAuth.Shared.PandaMadeScopes");
        Assert.NotNull(contract);
        Assert.Equal("pandamade.manage", contract.GetField("Manage")!.GetRawConstantValue());
        Assert.Equal("pandamade-api", contract.GetField("Api")!.GetRawConstantValue());
        Assert.Equal("pandamade-admin", contract.GetField("AdminClient")!.GetRawConstantValue());
        Assert.Equal("https://auth.pandamade.net/", contract.GetField("Issuer")!.GetRawConstantValue());
        Assert.DoesNotContain("pandamade.manage", PandaAuthScopes.FleetAll);
    }

    [Theory]
    [InlineData("ProductWebClient", "pandamade-web")]
    [InlineData("ProductApi", "pandamade-product-api")]
    [InlineData("ProductAccount", "pandamade.account")]
    [InlineData("ProductSignInCallback", "https://pandamade.net/auth/signin-oidc")]
    [InlineData("ProductSignOutCallback", "https://pandamade.net/auth/signout-callback-oidc")]
    public void ProductAccountContractHasDedicatedValues(string field, string expected)
    {
        var constant = typeof(PandaMadeScopes).GetField(field);
        Assert.NotNull(constant);
        Assert.Equal(expected, constant.GetRawConstantValue());
    }
}
