using System.Text.Json.Serialization;

namespace ADTO.DCloud.Desktop.Models;

public sealed record AuthenticateRequest(
    [property: JsonPropertyName("userName")] string UserName,
    [property: JsonPropertyName("password")] string Password);

public sealed record AuthenticateResponse(
    [property: JsonPropertyName("accessToken")] string? AccessToken,
    [property: JsonPropertyName("encryptedAccessToken")] string? EncryptedAccessToken,
    [property: JsonPropertyName("expireInSeconds")] int? ExpireInSeconds,
    [property: JsonPropertyName("userId")] long? UserId);

public sealed record ApiEnvelope<T>(
    [property: JsonPropertyName("result")] T? Result,
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("error")] ApiError? Error,
    [property: JsonPropertyName("unAuthorizedRequest")] bool UnAuthorizedRequest);

public sealed record ApiError(
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("details")] string? Details);

public sealed record UserSession(string UserName, string AccessToken, DateTimeOffset LoginTime, int? ExpireInSeconds, long? UserId);
