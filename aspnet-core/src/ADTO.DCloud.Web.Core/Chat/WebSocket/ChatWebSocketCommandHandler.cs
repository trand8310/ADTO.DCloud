using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ADTO.DCloud.Chat;
using ADTO.DCloud.Chat.Dto;
using ADTOSharp;
using ADTOSharp.Dependency;

namespace ADTO.DCloud.Web.Chat.WebSocket;

public class ChatWebSocketCommandHandler : IChatWebSocketCommandHandler, ITransientDependency
{
    private readonly IChatMessageManager _chatMessageManager;
    private readonly IChatWebSocketConnectionManager _connectionManager;

    public ChatWebSocketCommandHandler(IChatMessageManager chatMessageManager, IChatWebSocketConnectionManager connectionManager)
    {
        _chatMessageManager = chatMessageManager;
        _connectionManager = connectionManager;
    }

    public async Task HandleAsync(WebSocket socket, UserIdentifier sender, ChatWebSocketRequest request, string connectionId, CancellationToken cancellationToken)
    {
        switch (request.Action)
        {
            case "heartbeat":
                await SendToSocketAsync(socket, "pong", new { time = DateTime.UtcNow }, cancellationToken);
                return;
            case "joinGroup":
                var groupName = request.Data.GetProperty("groupName").GetString() ?? string.Empty;
                await _connectionManager.JoinGroupAsync(connectionId, groupName);
                await SendToSocketAsync(socket, "joinGroupAck", new { groupName }, cancellationToken);
                return;
            case "leaveGroup":
                var left = request.Data.GetProperty("groupName").GetString() ?? string.Empty;
                await _connectionManager.LeaveGroupAsync(connectionId, left);
                await SendToSocketAsync(socket, "leaveGroupAck", new { groupName = left }, cancellationToken);
                return;
            case "sendMessageToUser":
                var input = request.Data.Deserialize<SendChatMessageInput>();
                if (input == null) throw new InvalidOperationException("Invalid sendMessageToUser payload.");
                await _chatMessageManager.SendMessageAsync(sender, new UserIdentifier(input.TenantId, input.UserId), input.Message, input.TenancyName, input.UserName, input.ProfilePictureId);
                await SendToSocketAsync(socket, "sendMessageToUserAck", new { ok = true }, cancellationToken);
                return;
            default:
                await SendToSocketAsync(socket, "error", new { message = $"Unsupported action: {request.Action}" }, cancellationToken);
                return;
        }
    }

    private static Task SendToSocketAsync(WebSocket socket, string method, object payload, CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new ChatWebSocketResponse { Method = method, Payload = payload }));
        return socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
    }
}
