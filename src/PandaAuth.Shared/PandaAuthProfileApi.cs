using System.Text.Json;
using System.Text.Json.Serialization;

namespace PandaAuth.Shared;

/// <summary>Private self-profile contract. No caller-selected account or public projection.</summary>
public static class PandaAuthProfileApi
{
    public const string Self = "/api/profile/v1/self";
    public const string Avatar = Self + "/avatar";
    public const string ContactEmail = Self + "/contact-email";
    public const string ContactEmailChallenges = ContactEmail + "/challenges";
    public const string ContactEmailConfirmation = ContactEmail + "/confirmation";
    public const string SelfOrganization = Self + "/organizations/{organizationKey}";
    public const string OrganizationMember = "/api/profile/v1/organizations/{organizationKey}/members/{memberKey}";
    public const string OrganizationAvatar = OrganizationMember + "/avatar";
}

/// <summary>Only these profile fields can be explicitly shared with a registered organization.</summary>
public static class ProfileFields
{
    public const string DisplayName = "displayName";
    public const string Avatar = "avatar";
    public const string Biography = "biography";
    public const string ContactEmail = "contactEmail";
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ProfileOrganizationSelectionRequest(
    [property: JsonPropertyName("fields"), JsonRequired] string[] Fields);

public sealed record ProfileOrganizationSelectionDto(
    [property: JsonPropertyName("fields")] string[] Fields,
    [property: JsonPropertyName("revision")] string Revision);

/// <summary>Filtered audience view: no immutable account reference or private verification/revision metadata.</summary>
public sealed record OrganizationProfileDto(
    [property: JsonPropertyName("displayName"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? DisplayName,
    [property: JsonPropertyName("biography"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Biography,
    [property: JsonPropertyName("avatar"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] bool? Avatar,
    [property: JsonPropertyName("contactEmail"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? ContactEmail);

public static class ProfileScopes
{
    public const string Resource = "profile-api";
    public const string Read = "profile.read";
    public const string Write = "profile.write";
}

[JsonConverter(typeof(ProfileContactEmailStateConverter))]
public enum ProfileContactEmailState { None, Pending, Verified }

public sealed class ProfileContactEmailStateConverter()
    : JsonStringEnumConverter<ProfileContactEmailState>(JsonNamingPolicy.CamelCase, allowIntegerValues: false);

public sealed record ProfileContactEmailDto(
    [property: JsonPropertyName("address")] string? Address,
    [property: JsonPropertyName("state")] ProfileContactEmailState State);

public sealed record SelfProfileDto(
    [property: JsonPropertyName("displayName")] string? DisplayName,
    [property: JsonPropertyName("biography")] string? Biography,
    [property: JsonPropertyName("hasAvatar")] bool HasAvatar,
    [property: JsonPropertyName("contactEmail")] ProfileContactEmailDto ContactEmail,
    [property: JsonPropertyName("revision")] string Revision);

/// <summary>Undefined means absent; JSON null means explicitly supplied null. Server validates values and duplicates.</summary>
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ProfilePatchRequest
{
    [JsonPropertyName("displayName"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public JsonElement DisplayName { get; init; }

    [JsonPropertyName("biography"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public JsonElement Biography { get; init; }
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ProfileContactEmailChallengeRequest(
    [property: JsonPropertyName("address"), JsonRequired] string Address);

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ProfileContactEmailConfirmationRequest(
    [property: JsonPropertyName("challengeId"), JsonRequired] string ChallengeId,
    [property: JsonPropertyName("token"), JsonRequired] string Token);

public sealed record ProfileContactEmailChallengeResponse(
    [property: JsonPropertyName("profile")] SelfProfileDto Profile,
    [property: JsonPropertyName("challengeId")] string ChallengeId);

public sealed record ProfileErrorDto(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("correlationId")] string CorrelationId);
