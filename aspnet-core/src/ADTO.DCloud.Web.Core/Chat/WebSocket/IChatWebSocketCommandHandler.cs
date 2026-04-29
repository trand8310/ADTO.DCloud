using System.Net.WebSockets;
using ADTOSharp;

namespace ADTO.DCloud.Web.Chat.WebSocket;

public interface IChatWebSocketCommandHandler
{
    Task HandleAsync(WebSocket socket, UserIdentifier sender, ChatWebSocketRequest request, string connectionId, CancellationToken cancellationToken);
}
