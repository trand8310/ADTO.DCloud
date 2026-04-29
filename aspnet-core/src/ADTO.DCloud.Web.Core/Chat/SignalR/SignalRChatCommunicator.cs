using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Dependency;
using ADTOSharp.ObjectMapping;
using ADTOSharp.RealTime;
using Castle.Core.Logging;
using Microsoft.AspNetCore.SignalR;
using ADTO.DCloud.Chat;
using ADTO.DCloud.Chat.Dto;
using ADTO.DCloud.Friendships;
using ADTO.DCloud.Web.Chat.WebSocket;
using ADTO.DCloud.Friendships.Dto;

namespace ADTO.DCloud.Web.Chat.SignalR;

/// <summary>
/// 消息通讯,这里写的方法,对应客户端也需要支持,才可以做出响应
/// </summary>
public class SignalRChatCommunicator : IChatCommunicator, ITransientDependency
{
    /// <summary>
    /// Reference to the logger.
    /// </summary>
    public ILogger Logger { get; set; }

    private readonly IObjectMapper _objectMapper;

    private readonly IHubContext<ChatHub> _chatHub;
    private readonly IChatWebSocketConnectionManager _webSocketConnectionManager;

    public SignalRChatCommunicator(
        IObjectMapper objectMapper,
        IHubContext<ChatHub> chatHub,
        IChatWebSocketConnectionManager webSocketConnectionManager)
    {
        _objectMapper = objectMapper;
        _chatHub = chatHub;
        _webSocketConnectionManager = webSocketConnectionManager;
        Logger = NullLogger.Instance;
    }

    /// <summary>
    /// 发送消息给在线用户
    /// </summary>
    /// <param name="clients"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendMessageToClient(IReadOnlyList<IOnlineClient> clients, ChatMessage message)
    {
        //向所有在线的客户发送消息
        foreach (var client in clients)
        {
            var signalRClient = GetSignalRClientOrNull(client);
            if (signalRClient == null)
            {
                return;
            }

            var payload = _objectMapper.Map<ChatMessageDto>(message);
            await signalRClient.SendAsync("getChatMessage", payload);
            await _webSocketConnectionManager.SendAsync(client.UserId, "getChatMessage", payload);
        }
    }

    /// <summary>
    /// 发送消息给好友
    /// </summary>
    /// <param name="clients"></param>
    /// <param name="friendship"></param>
    /// <param name="isOwnRequest"></param>
    /// <param name="isFriendOnline"></param>
    /// <returns></returns>
    public async Task SendFriendshipRequestToClient(IReadOnlyList<IOnlineClient> clients, Friendship friendship, bool isOwnRequest, bool isFriendOnline)
    {
        foreach (var client in clients)
        {
            var signalRClient = GetSignalRClientOrNull(client);
            if (signalRClient == null)
            {
                return;
            }

            var friendshipRequest = _objectMapper.Map<FriendDto>(friendship);
            friendshipRequest.IsOnline = isFriendOnline;

            await signalRClient.SendAsync("getFriendshipRequest", friendshipRequest, isOwnRequest);
            await _webSocketConnectionManager.SendAsync(client.UserId, "getFriendshipRequest", new { friendshipRequest, isOwnRequest });
        }
    }

    /// <summary>
    /// 发送用户链接状态给指定用户
    /// </summary>
    /// <param name="clients"></param>
    /// <param name="user"></param>
    /// <param name="isConnected"></param>
    /// <returns></returns>
    public async Task SendUserConnectionChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user, bool isConnected)
    {
        foreach (var client in clients)
        {
            var signalRClient = GetSignalRClientOrNull(client);
            if (signalRClient == null)
            {
                continue;
            }

            await signalRClient.SendAsync("getUserConnectNotification", user, isConnected);
            await _webSocketConnectionManager.SendAsync(client.UserId, "getUserConnectNotification", new { user, isConnected });
        }
    }

    public async Task SendUserStateChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user, FriendshipState newState)
    {
        foreach (var client in clients)
        {
            var signalRClient = GetSignalRClientOrNull(client);
            if (signalRClient == null)
            {
                continue;
            }

            await signalRClient.SendAsync("getUserStateChange", user, newState);
            await _webSocketConnectionManager.SendAsync(client.UserId, "getUserStateChange", new { user, newState });
        }
    }

    /// <summary>
    /// 通知所有用户,的未读消息
    /// </summary>
    /// <param name="clients"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task SendAllUnreadMessagesOfUserReadToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user)
    {
        foreach (var client in clients)
        {
            var signalRClient = GetSignalRClientOrNull(client);
            if (signalRClient == null)
            {
                continue;
            }

            await signalRClient.SendAsync("getallUnreadMessagesOfUserRead", user);
            await _webSocketConnectionManager.SendAsync(client.UserId, "getallUnreadMessagesOfUserRead", user);
        }
    }

    /// <summary>
    /// 发送阅读状态给用户
    /// </summary>
    /// <param name="clients"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task SendReadStateChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user)
    {
        foreach (var client in clients)
        {
            var signalRClient = GetSignalRClientOrNull(client);
            if (signalRClient == null)
            {
                continue;
            }

            await signalRClient.SendAsync("getReadStateChange", user);
            await _webSocketConnectionManager.SendAsync(client.UserId, "getReadStateChange", user);
        }
    }

    private IClientProxy GetSignalRClientOrNull(IOnlineClient client)
    {
        var signalRClient = _chatHub.Clients.Client(client.ConnectionId);
        if (signalRClient == null)
        {
            Logger.Debug($"无法从SignalR hub获取聊天用户:{client.UserId}" );
            return null;
        }

        return signalRClient;
    }

}