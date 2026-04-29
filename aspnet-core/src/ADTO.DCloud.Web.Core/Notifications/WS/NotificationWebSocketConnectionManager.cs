using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Dependency;

namespace ADTO.DCloud.Notifications.WS;

public class NotificationWebSocketConnectionManager : INotificationWebSocketConnectionManager, ISingletonDependency
{
    private readonly ConcurrentDictionary<string, (WebSocket Socket, UserIdentifier User)> _connections = new();
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _userConnections = new();

    public Task<string> AddConnectionAsync(WebSocket socket, UserIdentifier user)
    {
        var connectionId = Guid.NewGuid().ToString("N");
        _connections[connectionId] = (socket, user);
        var userKey = GetUserKey(user);
        var userConnectionIds = _userConnections.GetOrAdd(userKey, _ => new ConcurrentDictionary<string, byte>());
        userConnectionIds[connectionId] = 0;
        return Task.FromResult(connectionId);
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        if (!_connections.TryRemove(connectionId, out var entry))
        {
            return;
        }

        if (entry.Socket.State == WebSocketState.Open)
        {
            await entry.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closed", CancellationToken.None);
        }

        if (entry.User != null)
        {
            var userKey = GetUserKey(entry.User);
            if (_userConnections.TryGetValue(userKey, out var userConnectionIds))
            {
                userConnectionIds.TryRemove(connectionId, out _);
                if (userConnectionIds.IsEmpty)
                {
                    _userConnections.TryRemove(userKey, out _);
                }
            }
        }
    }

    public async Task SendAsync(UserIdentifier user, string method, object payload)
    {
        var body = JsonSerializer.Serialize(new { method, payload });
        var buffer = Encoding.UTF8.GetBytes(body);
        var userKey = GetUserKey(user);
        if (!_userConnections.TryGetValue(userKey, out var userConnectionIds))
        {
            return;
        }

        foreach (var connectionId in userConnectionIds.Keys)
        {
            if (!_connections.TryGetValue(connectionId, out var connection))
            {
                userConnectionIds.TryRemove(connectionId, out _);
                continue;
            }

            if (connection.Socket.State == WebSocketState.Open)
            {
                await connection.Socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                continue;
            }

            userConnectionIds.TryRemove(connectionId, out _);
        }

        if (userConnectionIds.IsEmpty)
        {
            _userConnections.TryRemove(userKey, out _);
        }
    }

    private static string GetUserKey(UserIdentifier user)
    {
        return $"{user.TenantId?.ToString() ?? "host"}:{user.UserId}";
    }
}
