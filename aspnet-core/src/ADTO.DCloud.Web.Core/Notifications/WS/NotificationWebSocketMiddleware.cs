using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Runtime.Session;
using Microsoft.AspNetCore.Http;

namespace ADTO.DCloud.Notifications.WS;

public class NotificationWebSocketMiddleware
{
    private readonly RequestDelegate _next;

    public NotificationWebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, INotificationWebSocketConnectionManager manager, IADTOSharpSession session)
    {
        if (!context.Request.Path.Equals("/ws-notification") || !context.WebSockets.IsWebSocketRequest)
        {
            await _next(context);
            return;
        }

        if (!session.UserId.HasValue)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        using var socket = await context.WebSockets.AcceptWebSocketAsync();
        var sender = new UserIdentifier(session.TenantId, session.UserId.Value);
        var connectionId = await manager.AddConnectionAsync(socket, sender);

        var buffer = new byte[1024];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), context.RequestAborted);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            if (string.Equals(message, "ping", StringComparison.OrdinalIgnoreCase))
            {
                var pong = Encoding.UTF8.GetBytes("pong");
                await socket.SendAsync(new ArraySegment<byte>(pong), WebSocketMessageType.Text, true, context.RequestAborted);
            }
        }

        await manager.RemoveConnectionAsync(connectionId);
    }
}
