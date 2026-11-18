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
        Assert.False(json.RootElement.GetProperty("contactEmail").TryGetProperty("pendingVerification", out _));
    }

    [Fact]
    public void Pending_contact_verification_serializes_only_the_locator_and_expiry()
    {
        var expires = new DateTimeOffset(2026, 11, 18, 10, 0, 0, TimeSpan.Zero);
        var id = Guid.Parse("aebd84ff-3eb2-4325-aa24-f34108d8c231");
        var contact = new ProfileContactEmailDto("owner@example.invalid", ProfileContactEmailState.Pending,
            new PendingContactVerificationDto(id, expires));
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(contact));
        var pending = json.RootElement.GetProperty("pendingVerification");

        Assert.Equal(["challengeId", "expiresAt"], pending.EnumerateObject().Select(property => property.Name));
        Assert.Equal(id, pending.GetProperty("challengeId").GetGuid());
        Assert.Equal(expires, pending.GetProperty("expiresAt").GetDateTimeOffset());
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
