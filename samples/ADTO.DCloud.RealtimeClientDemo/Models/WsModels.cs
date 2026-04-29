namespace ADTO.DCloud.RealtimeClientDemo.Models;

public class ChatWebSocketRequest
{
    public string Action { get; set; } = string.Empty;
    public object? Data { get; set; }
}
