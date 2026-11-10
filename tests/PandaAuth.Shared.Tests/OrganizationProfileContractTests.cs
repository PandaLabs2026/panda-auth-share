using System.Text.Json;
using Xunit;

namespace PandaAuth.Shared.Tests;

public class OrganizationProfileContractTests
{
    [Fact]
    public void Filtered_projection_omits_hidden_values_and_preserves_authorized_avatar_absence()
    {
        var json = JsonSerializer.Serialize(new OrganizationProfileDto(null, "Visible bio", false, null));
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        Assert.Equal(2, root.EnumerateObject().Count());
        Assert.Equal("Visible bio", root.GetProperty("biography").GetString());
        Assert.False(root.GetProperty("avatar").GetBoolean());
        Assert.False(root.TryGetProperty("contactEmail", out _));
        Assert.False(root.TryGetProperty("displayName", out _));
    }

    [Fact]
    public void Selection_replacement_does_not_accept_caller_identity_or_membership_claims()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ProfileOrganizationSelectionRequest>(
            "{\"fields\":[\"avatar\"],\"subject\":\"other-account\"}"));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ProfileOrganizationSelectionRequest>("{}"));
    }
}
