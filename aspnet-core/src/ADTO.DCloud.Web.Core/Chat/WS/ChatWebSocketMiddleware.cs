using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Runtime.Session;
using Microsoft.AspNetCore.Http;

namespace ADTO.DCloud.Chat.WS;

public class ChatWebSocketMiddleware
{
    private readonly RequestDelegate _next;

    public ChatWebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IChatWebSocketConnectionManager manager, IADTOSharpSession session, IChatWebSocketCommandHandler commandHandler)
    {
        if (!context.Request.Path.Equals("/ws-chat") || !context.WebSockets.IsWebSocketRequest)
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

        var buffer = new byte[4096];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), context.RequestAborted);
            if (result.MessageType == WebSocketMessageType.Close) break;

            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            ChatWebSocketRequest? request = null;
            try
            {
                request = JsonSerializer.Deserialize<ChatWebSocketRequest>(json);
            }
            catch
            {
                // ignore invalid JSON
            }

            if (request == null || string.IsNullOrWhiteSpace(request.Action))
            {
                var invalid = Encoding.UTF8.GetBytes("{\"method\":\"error\",\"payload\":{\"message\":\"Invalid request.\"}}");
                await socket.SendAsync(new ArraySegment<byte>(invalid), WebSocketMessageType.Text, true, context.RequestAborted);
                continue;
            }

            await commandHandler.HandleAsync(socket, sender, request, connectionId, context.RequestAborted);
        }

        await manager.RemoveConnectionAsync(connectionId);
    }
}
