using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ADTOSharp;
using ADTOSharp.Dependency;

namespace ADTO.DCloud.Web.Chat.WebSocket;

public class ChatWebSocketConnectionManager : IChatWebSocketConnectionManager, ISingletonDependency
{
    private readonly ConcurrentDictionary<string, (WebSocket Socket, UserIdentifier User)> _connections = new();
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _groups = new();

    public Task<string> AddConnectionAsync(WebSocket socket, UserIdentifier user)
    {
        var connectionId = Guid.NewGuid().ToString("N");
        _connections[connectionId] = (socket, user);
        return Task.FromResult(connectionId);
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        foreach (var group in _groups.Values)
        {
            group.TryRemove(connectionId, out _);
        }

        if (_connections.TryRemove(connectionId, out var entry) && entry.Socket.State == WebSocketState.Open)
        {
            await entry.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closed", CancellationToken.None);
        }
    }

    public Task JoinGroupAsync(string connectionId, string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName)) return Task.CompletedTask;
        var group = _groups.GetOrAdd(groupName, _ => new ConcurrentDictionary<string, byte>());
        group[connectionId] = 0;
        return Task.CompletedTask;
    }

    public Task LeaveGroupAsync(string connectionId, string groupName)
    {
        if (_groups.TryGetValue(groupName, out var group))
        {
            group.TryRemove(connectionId, out _);
        }
        return Task.CompletedTask;
    }

    public Task SendAsync(UserIdentifier user, string method, object payload)
    {
        return SendByFilterAsync(x => x.Value.User.UserId == user.UserId && x.Value.User.TenantId == user.TenantId, method, payload);
    }

    public Task SendGroupAsync(string groupName, string method, object payload)
    {
        if (!_groups.TryGetValue(groupName, out var group) || group.Count == 0) return Task.CompletedTask;
        return SendByFilterAsync(x => group.ContainsKey(x.Key), method, payload);
    }

    public Task SendAllAsync(string method, object payload)
    {
        return SendByFilterAsync(_ => true, method, payload);
    }

    private async Task SendByFilterAsync(Func<KeyValuePair<string, (WebSocket Socket, UserIdentifier User)>, bool> filter, string method, object payload)
    {
        var body = JsonSerializer.Serialize(new ChatWebSocketResponse { Method = method, Payload = payload });
        var buffer = Encoding.UTF8.GetBytes(body);

        foreach (var item in _connections.Where(filter))
        {
            if (item.Value.Socket.State == WebSocketState.Open)
            {
                await item.Value.Socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
