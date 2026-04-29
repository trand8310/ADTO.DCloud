namespace ADTO.DCloud.RealtimeClientDemo.Models;

public class AuthRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthEnvelope
{
    public AuthResult? Result { get; set; }
}

public class AuthResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string EncryptedAccessToken { get; set; } = string.Empty;
    public long ExpireInSeconds { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool Success { get; set; }
}

public class ClientSettings
{
    public string ApiBaseUrl { get; set; } = "http://localhost:44380/";
}
