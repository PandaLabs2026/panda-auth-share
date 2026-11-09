using System.Text.Json;
using PandaAuth.Shared;
using Xunit;

namespace PandaAuth.Shared.Tests;

public sealed class SelfProfileContractTests
{
    [Fact]
    public void Self_projection_uses_exact_private_wire_shape()
    {
        var dto = new SelfProfileDto(null, null, false,
            new ProfileContactEmailDto(null, ProfileContactEmailState.None), "\"0\"");
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(dto));
        Assert.Equal(["displayName", "biography", "hasAvatar", "contactEmail", "revision"],
            json.RootElement.EnumerateObject().Select(p => p.Name));
        Assert.Equal(JsonValueKind.Null, json.RootElement.GetProperty("displayName").ValueKind);
        Assert.Equal("none", json.RootElement.GetProperty("contactEmail").GetProperty("state").GetString());
        Assert.Equal("\"0\"", json.RootElement.GetProperty("revision").GetString());
    }

    [Fact]
    public void Patch_preserves_missing_versus_explicit_null()
    {
        var request = JsonSerializer.Deserialize<ProfilePatchRequest>("{\"biography\":null}")!;
        Assert.Equal(JsonValueKind.Undefined, request.DisplayName.ValueKind);
        Assert.Equal(JsonValueKind.Null, request.Biography.ValueKind);
        Assert.Equal("{\"biography\":null}", JsonSerializer.Serialize(request));
    }

    [Theory]
    [InlineData("{\"subject\":\"other\"}")]
    [InlineData("{\"roles\":[\"admin\"]}")]
    public void Patch_rejects_immutable_or_unknown_input(string json)
        => Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ProfilePatchRequest>(json));

    [Theory]
    [InlineData("{\"address\":\"test@example.invalid\",\"userId\":\"other\"}")]
    [InlineData("{}")]
    public void Challenge_creation_rejects_target_identity_and_missing_address(string json)
        => Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ProfileContactEmailChallengeRequest>(json));

    [Theory]
    [InlineData("{\"address\":null,\"state\":2}")]
    [InlineData("{\"address\":null,\"state\":\"unknown\"}")]
    public void Contact_state_does_not_accept_numeric_or_unknown_values(string json)
        => Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ProfileContactEmailDto>(json));
}
