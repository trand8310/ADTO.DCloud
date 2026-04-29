
using ADTOSharp;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ADTO.DCloud.Chat.WS;

public interface IChatWebSocketCommandHandler
{
    Task HandleAsync(WebSocket socket, UserIdentifier sender, ChatWebSocketRequest request, string connectionId, CancellationToken cancellationToken);
}
