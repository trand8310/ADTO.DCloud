using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTO.DCloud.Chat.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.Runtime.Session;
using System;
using ADTOSharp.Domain.Repositories;
using ADTO.DCloud.Friendships.Cache;
using ADTOSharp.RealTime;
using ADTO.DCloud.Friendships.Dto;
using ADTOSharp;
using ADTOSharp.Timing;
using ADTOSharp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;


namespace ADTO.DCloud.Chat;

/// <summary>
/// 基于SignalR的实时通讯服务
/// </summary>
[ADTOSharpAuthorize]
public class ChatAppService : DCloudAppServiceBase, IChatAppService
{
    #region Fields
    private readonly IRepository<ChatMessage, long> _chatMessageRepository;
    private readonly IUserFriendsCache _userFriendsCache;
    private readonly IOnlineClientManager _onlineClientManager;
    private readonly IChatCommunicator _chatCommunicator;
    #endregion

    #region Ctor
    public ChatAppService(
        IRepository<ChatMessage, long> chatMessageRepository,
        IUserFriendsCache userFriendsCache,
        IOnlineClientManager onlineClientManager,
        IChatCommunicator chatCommunicator)
    {
        _chatMessageRepository = chatMessageRepository;
        _userFriendsCache = userFriendsCache;
        _onlineClientManager = onlineClientManager;
        _chatCommunicator = chatCommunicator;
    }
    #endregion

    #region Methods
    /// <summary>
    /// 获取当前用户的好友列表
    /// </summary>
    /// <returns></returns>
    [DisableAuditing, HttpPost]
    public async Task<GetUserChatFriendsWithSettingsOutput> GetUserChatFriendsWithSettings()
    {
        var userIdentifier = ADTOSharpSession.ToUserIdentifier();
        if (userIdentifier == null)
        {
            return new GetUserChatFriendsWithSettingsOutput();
        }

        var cacheItem = _userFriendsCache.GetCacheItem(userIdentifier);
        var friends = ObjectMapper.Map<List<FriendDto>>(cacheItem.Friends);

        foreach (var friend in friends)
        {
            friend.IsOnline = await _onlineClientManager.IsOnlineAsync(
                new UserIdentifier(friend.FriendTenantId, friend.FriendUserId)
            );
        }

        return new GetUserChatFriendsWithSettingsOutput
        {
            Friends = friends,
            ServerTime = Clock.Now
        };
    }


    /// <summary>
    /// 获取我的在线好友
    /// </summary>
    [DisableAuditing, HttpPost]
    public async Task<GetUserChatFriendsWithSettingsOutput> GetMyOnlineFriends()
    {
        var userIdentifier = ADTOSharpSession.ToUserIdentifier();
        if (userIdentifier == null)
        {
            return new GetUserChatFriendsWithSettingsOutput();
        }

        var cacheItem = _userFriendsCache.GetCacheItem(userIdentifier);
        var friends = ObjectMapper.Map<List<FriendDto>>(cacheItem.Friends);

        var onlineFriends = new List<FriendDto>();

        foreach (var friend in friends)
        {
            var isOnline = await _onlineClientManager.IsOnlineAsync(
                new UserIdentifier(friend.FriendTenantId, friend.FriendUserId)
            );

            if (isOnline)
            {
                friend.IsOnline = true;
                onlineFriends.Add(friend);
            }
        }

        return new GetUserChatFriendsWithSettingsOutput
        {
            Friends = onlineFriends,
            ServerTime = Clock.Now
        };
    }

    /// <summary>
    /// 获取所有在线用户
    /// 注意：如果一个用户有多个连接，会返回多条记录。
    /// 如果你只想每个用户一条，请看下方去重版写法。
    /// </summary>
    [DisableAuditing,HttpPost]
    public async Task<ListResultDto<OnlineUserDto>> GetOnlineUsers()
    {
        var clients = await _onlineClientManager.GetAllClientsAsync();

        var result = clients
            .Where(x => x.UserId.HasValue)
            .GroupBy(x => new { x.TenantId, x.UserId })
            .Select(g =>
            {
                var first = g.First();
                return new OnlineUserDto
                {
                    TenantId = first.TenantId,
                    UserId = first.UserId!.Value,
                    UserName =   "",
                    TenancyName = "",
                    ConnectionId = first.ConnectionId
                };
            })
            .OrderBy(x => x.TenancyName)
            .ThenBy(x => x.UserName)
            .ToList();

        return new ListResultDto<OnlineUserDto>(result);
    }

    /// <summary>
    /// 判断指定用户是否在线
    /// </summary>
    [DisableAuditing,HttpPost]
    public async Task<bool> IsUserOnline(Guid? tenantId, Guid userId)
    {
        return await _onlineClientManager.IsOnlineAsync(new UserIdentifier(tenantId, userId));
    }


    /// <summary>
    /// 我的在线好友数量
    /// </summary>
    /// <returns></returns>
    [DisableAuditing, HttpPost]
    public async Task<int> GetMyOnlineFriendsCount()
    {
        var userIdentifier = ADTOSharpSession.ToUserIdentifier();
        if (userIdentifier == null)
        {
            return 0;
        }

        var cacheItem = _userFriendsCache.GetCacheItem(userIdentifier);
        var friends = cacheItem.Friends;

        var count = 0;

        foreach (var friend in friends)
        {
            var isOnline = await _onlineClientManager.IsOnlineAsync(
                new UserIdentifier(friend.FriendTenantId, friend.FriendUserId)
            );

            if (isOnline)
            {
                count++;
            }
        }

        return count;
    }


    /// <summary>
    /// 获取当前用户的消息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisableAuditing, HttpPost]
    public async Task<ListResultDto<ChatMessageDto>> GetUserChatMessages(GetUserChatMessagesInput input)
    {
        var userId = ADTOSharpSession.GetUserId();
        var messages = await _chatMessageRepository.GetAll()
                .WhereIf(input.MinMessageId.HasValue, m => m.Id < input.MinMessageId.Value)
                .Where(m => m.UserId == userId && m.TargetTenantId == input.TenantId && m.TargetUserId == input.UserId)
                .OrderByDescending(m => m.CreationTime)
                .Take(input.MaxResultCount)
                .ToListAsync();

        messages.Reverse();

        return new ListResultDto<ChatMessageDto>(ObjectMapper.Map<List<ChatMessageDto>>(messages));
    }
    /// <summary>
    /// 获取用户所有的未读消息,并发送给用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task MarkAllUnreadMessagesOfUserAsRead(MarkAllUnreadMessagesOfUserAsReadInput input)
    {
        var userId = ADTOSharpSession.GetUserId();
        var tenantId = ADTOSharpSession.TenantId;

        // receiver messages
        var messages = await _chatMessageRepository
             .GetAll()
             .Where(m =>
                    m.UserId == userId &&
                    m.TargetTenantId == input.TenantId &&
                    m.TargetUserId == input.UserId &&
                    m.ReadState == ChatMessageReadState.Unread)
             .ToListAsync();

        if (!messages.Any())
        {
            return;
        }

        foreach (var message in messages)
        {
            message.ChangeReadState(ChatMessageReadState.Read);
        }

        // sender messages
        using (CurrentUnitOfWork.SetTenantId(input.TenantId))
        {
            var reverseMessages = await _chatMessageRepository.GetAll()
                .Where(m => m.UserId == input.UserId && m.TargetTenantId == tenantId && m.TargetUserId == userId)
                .ToListAsync();

            if (!reverseMessages.Any())
            {
                return;
            }

            foreach (var message in reverseMessages)
            {
                message.ChangeReceiverReadState(ChatMessageReadState.Read);
            }
        }

        var userIdentifier = ADTOSharpSession.ToUserIdentifier();
        var friendIdentifier = input.ToUserIdentifier();

        _userFriendsCache.ResetUnreadMessageCount(userIdentifier, friendIdentifier);

        var onlineUserClients = await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
        if (onlineUserClients.Any())
        {
            await _chatCommunicator.SendAllUnreadMessagesOfUserReadToClients(onlineUserClients, friendIdentifier);
        }

        var onlineFriendClients = await _onlineClientManager.GetAllByUserIdAsync(friendIdentifier);
        if (onlineFriendClients.Any())
        {
            await _chatCommunicator.SendReadStateChangeToClients(onlineFriendClients, userIdentifier);
        }
    }
    #endregion
}
