using System.Text.Json;


namespace ADTO.DCloud.Chat.WS;

public class ChatWebSocketRequest
{
    public string Action { get; set; } = string.Empty;
    public JsonElement Data { get; set; }
}

public class ChatWebSocketResponse
{
    public string Method { get; set; } = string.Empty;
    public object? Payload { get; set; }
}
