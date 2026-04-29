using System.Text.Json;
using ADTO.DCloud.Chat.Dto;

namespace ADTO.DCloud.Web.Chat.WebSocket;

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
