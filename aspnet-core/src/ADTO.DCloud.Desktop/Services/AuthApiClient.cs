using System.Net.Http.Json;
using System.Text.Json;
using ADTO.DCloud.Desktop.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ADTO.DCloud.Desktop.Services;

public sealed class AuthApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<AuthApiClient> logger) : IAuthApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<UserSession> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        var baseUrl = configuration["Api:BaseUrl"] ?? "http://localhost:44380/";
        var authenticatePath = configuration["Api:AuthenticatePath"] ?? "api/TokenAuth/Authenticate";

        var endpoint = new Uri(new Uri(baseUrl, UriKind.Absolute), authenticatePath);
        logger.LogInformation("Authenticating desktop user {UserName} against {Endpoint}", userName, endpoint);

        using var response = await httpClient.PostAsJsonAsync(endpoint, new AuthenticateRequest(userName, password), JsonOptions, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"登录失败：HTTP {(int)response.StatusCode} {response.ReasonPhrase}。{payload}");
        }

        var envelope = JsonSerializer.Deserialize<ApiEnvelope<AuthenticateResponse>>(payload, JsonOptions);
        if (envelope is null)
        {
            throw new InvalidOperationException("登录失败：服务端返回为空或格式无法识别。");
        }

        if (!envelope.Success || envelope.Result is null)
        {
            throw new InvalidOperationException(envelope.Error?.Message ?? "登录失败：账号、密码或租户状态异常。");
        }

        if (string.IsNullOrWhiteSpace(envelope.Result.AccessToken))
        {
            throw new InvalidOperationException("登录失败：服务端未返回 accessToken。");
        }

        return new UserSession(userName, envelope.Result.AccessToken, DateTimeOffset.Now, envelope.Result.ExpireInSeconds, envelope.Result.UserId);
    }
}
