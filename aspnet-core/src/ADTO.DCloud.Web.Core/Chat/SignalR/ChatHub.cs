using ADTO.DCloud.Chat;
using ADTO.DCloud.Chat.Dto;
using ADTO.DCloud.Web.Xss;
using ADTOSharp;
using ADTOSharp.AspNetCore.SignalR.Hubs;
using ADTOSharp.Localization;
using ADTOSharp.RealTime;
using ADTOSharp.Runtime.Caching.Redis.RealTime;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Castle.Core.Logging;
using Castle.Windsor;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using ADTO.DCloud.Chat.WS;

namespace ADTO.DCloud.Web.Chat.SignalR;
/// <summary>
/// 消息HUB
/// </summary>
public class ChatHub : OnlineClientHubBase
{
    private readonly IChatMessageManager _chatMessageManager;
    private readonly ILocalizationManager _localizationManager;
    private readonly IWindsorContainer _windsorContainer;
    private readonly IHtmlSanitizer _htmlSanitizer;
    private readonly IOnlineClientStore _onlineClientStore;
    private readonly IChatWebSocketConnectionManager _chatWebSocketConnectionManager;
    private bool _isCallByRelease;
    private IADTOSharpSession ChatAppSession { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatHub"/> class.
    /// </summary>
    public ChatHub(
        IChatMessageManager chatMessageManager,
        ILocalizationManager localizationManager,
        IWindsorContainer windsorContainer,
        IOnlineClientManager onlineClientManager,
        IOnlineClientInfoProvider clientInfoProvider, 
        IHtmlSanitizer htmlSanitizer,
        IOnlineClientStore onlineClientStore,
        IChatWebSocketConnectionManager chatWebSocketConnectionManager) : base(onlineClientManager, clientInfoProvider)
    {
        _chatMessageManager = chatMessageManager;
        _localizationManager = localizationManager;
        _windsorContainer = windsorContainer;
        _htmlSanitizer = htmlSanitizer;
        _onlineClientStore = onlineClientStore;
        _chatWebSocketConnectionManager = chatWebSocketConnectionManager;
        Logger = NullLogger.Instance;
        ChatAppSession = NullADTOSharpSession.Instance;

    }

    /// <summary>
    /// 客户端心跳（用于刷新 Redis 在线连接 TTL）。
    /// </summary>
    public async Task Heartbeat()
    {
        if (_onlineClientStore is RedisOnlineClientStore redisStore)
        {
            await redisStore.RefreshHeartbeatAsync(Context.ConnectionId);
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<string> SendMessage(SendChatMessageInput input)
    {
        input.Message = _htmlSanitizer.Sanitize(input.Message);
        var sender = Context.ToUserIdentifier();
        var receiver = new UserIdentifier(input.TenantId, input.UserId);

        try
        {
            using (ChatAppSession.Use(Context.GetTenantId(), Context.GetUserId()))
            {
                await _chatMessageManager.SendMessageAsync(sender, receiver, input.Message, input.TenancyName, input.UserName, input.ProfilePictureId);
                return string.Empty;
            }
        }
        catch (UserFriendlyException ex)
        {
            Logger.Warn("无法向用户发送聊天消息: " + receiver);
            Logger.Warn(ex.ToString(), ex);
            return ex.Message;
        }
        catch (Exception ex)
        {
            Logger.Warn("无法向用户发送聊天消息: " + receiver);
            Logger.Warn(ex.ToString(), ex);
            return _localizationManager.GetSource("DCloud").GetString("InternalServerError");
        }
    }


    /// <summary>
    /// 给指定用户发送消息（和 SendMessage 语义一致，更直观）
    /// </summary>
    public async Task<string> SendMessageToUser(SendChatMessageInput input)
    {
        return await SendMessage(input);
    }

    /// <summary>
    /// 加入群组
    /// </summary>
    public async Task<string> JoinGroup(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return "groupName 不能为空";
        }

        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Logger.Debug($"连接 {Context.ConnectionId} 已加入群组 {groupName}");
            return string.Empty;
        }
        catch (Exception ex)
        {
            Logger.Warn($"加入群组失败: {groupName}");
            Logger.Warn(ex.ToString(), ex);
            return _localizationManager.GetSource("DCloud").GetString("InternalServerError");
        }
    }

    /// <summary>
    /// 离开群组
    /// </summary>
    public async Task<string> LeaveGroup(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return "groupName 不能为空";
        }

        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            Logger.Debug($"连接 {Context.ConnectionId} 已离开群组 {groupName}");
            return string.Empty;
        }
        catch (Exception ex)
        {
            Logger.Warn($"离开群组失败: {groupName}");
            Logger.Warn(ex.ToString(), ex);
            return _localizationManager.GetSource("DCloud").GetString("InternalServerError");
        }
    }

    /// <summary>
    /// 发送群组消息（实时推送版）
    /// </summary>
    public async Task<string> SendMessageToGroup(SendGroupMessageInput input)
    {
        if (string.IsNullOrWhiteSpace(input.GroupName))
        {
            return "GroupName 不能为空";
        }

        if (string.IsNullOrWhiteSpace(input.Message))
        {
            return "消息不能为空";
        }

        try
        {
            input.Message = _htmlSanitizer.Sanitize(input.Message);

            var senderUserId = Context.GetUserId();
            var senderTenantId = Context.GetTenantId();

            var dto = new GroupMessageDto
            {
                GroupName = input.GroupName,
                Message = input.Message,
                SenderUserName = input.SenderUserName,
                SenderUserId = senderUserId,
                SenderTenantId = senderTenantId,
                CreationTime = DateTime.Now
            };

            await Clients.Group(input.GroupName).SendAsync("ReceiveGroupMessage", dto);
            await _chatWebSocketConnectionManager.SendGroupAsync(input.GroupName, "ReceiveGroupMessage", dto);

            return string.Empty;
        }
        catch (Exception ex)
        {
            Logger.Warn($"无法向群组发送消息: {input.GroupName}");
            Logger.Warn(ex.ToString(), ex);
            return _localizationManager.GetSource("DCloud").GetString("InternalServerError");
        }
    }


    /// <summary>
    /// 广播消息给所有在线连接
    /// </summary>
    public async Task<string> BroadcastMessage(BroadcastMessageInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Message))
        {
            return "消息不能为空";
        }

        try
        {
            input.Message = _htmlSanitizer.Sanitize(input.Message);

            var dto = new BroadcastMessageDto
            {
                Message = input.Message,
                SenderUserName = input.SenderUserName,
                SenderUserId = Context.GetUserId(),
                SenderTenantId = Context.GetTenantId(),
                CreationTime = DateTime.Now
            };

            await Clients.All.SendAsync("ReceiveBroadcastMessage", dto);
            await _chatWebSocketConnectionManager.SendAllAsync("ReceiveBroadcastMessage", dto);

            return string.Empty;
        }
        catch (Exception ex)
        {
            Logger.Warn("广播消息失败");
            Logger.Warn(ex.ToString(), ex);
            return _localizationManager.GetSource("DCloud").GetString("InternalServerError");
        }
    }


    /// <summary>
    /// 发送系统消息给指定用户（实时推送版）
    /// </summary>
    public async Task<string> SendSystemMessage(SendSystemMessageInput input)
    {
        try
        {
            input.Title = _htmlSanitizer.Sanitize(input.Title ?? "");
            input.Message = _htmlSanitizer.Sanitize(input.Message ?? "");

            var receiver = new UserIdentifier(input.TenantId, input.UserId);
            var receiverClients = await OnlineClientManager.GetAllByUserIdAsync(receiver);

            if (!receiverClients.Any())
            {
                return "目标用户当前不在线";
            }

            var dto = new SystemMessageDto
            {
                Title = input.Title,
                Message = input.Message,
                Level = string.IsNullOrWhiteSpace(input.Level) ? "info" : input.Level,
                CreationTime = DateTime.Now
            };

            foreach (var client in receiverClients)
            {
                await Clients.Client(client.ConnectionId)
                    .SendAsync("ReceiveSystemMessage", dto);
                await _chatWebSocketConnectionManager.SendAsync(receiver, "ReceiveSystemMessage", dto);
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            Logger.Warn($"发送系统消息失败: TenantId={input.TenantId}, UserId={input.UserId}");
            Logger.Warn(ex.ToString(), ex);
            return _localizationManager.GetSource("DCloud").GetString("InternalServerError");
        }
    }



    public void Register()
    {
        Logger.Debug("客户已注册: " + Context.ConnectionId);
    }

    protected override void Dispose(bool disposing)
    {
        if (_isCallByRelease)
        {
            return;
        }
        base.Dispose(disposing);
        if (disposing)
        {
            _isCallByRelease = true;
            _windsorContainer.Release(this);
        }
    }
}
