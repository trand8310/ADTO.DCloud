using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Chat;
using ADTO.DCloud.Chat.Dto;
using ADTO.DCloud.Friendships.Dto;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Extensions;
using ADTOSharp.MultiTenancy;
using ADTOSharp.RealTime;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.Friendships;

/// <summary>
/// 好友服务
/// </summary>
[ADTOSharpAuthorize]
public class FriendshipAppService : DCloudAppServiceBase, IFriendshipAppService
{
    #region Fields
    private readonly IFriendshipManager _friendshipManager;
    private readonly IOnlineClientManager _onlineClientManager;
    private readonly IChatCommunicator _chatCommunicator;
    private readonly ITenantCache _tenantCache;
    private readonly IChatFeatureChecker _chatFeatureChecker;
    #endregion

    #region Ctor

    public FriendshipAppService(
        IFriendshipManager friendshipManager,
        IOnlineClientManager onlineClientManager,
        IChatCommunicator chatCommunicator,
        ITenantCache tenantCache,
        IChatFeatureChecker chatFeatureChecker)
    {
        _friendshipManager = friendshipManager;
        _onlineClientManager = onlineClientManager;
        _chatCommunicator = chatCommunicator;
        _tenantCache = tenantCache;
        _chatFeatureChecker = chatFeatureChecker;
    }
    #endregion

    #region Utilities

    private async Task<string> GetTenancyNameAsync(Guid? tenantId)
    {
        if (tenantId.HasValue)
        {
            var tenant = await _tenantCache.GetAsync(tenantId.Value);
            return tenant.TenancyName;
        }

        return null;
    }

    /// <summary>
    /// 获取用户标识
    /// </summary>
    /// <param name="tenancyName"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    private async Task<UserIdentifier> GetUserIdentifier(string tenancyName, string userName)
    {
        Guid? tenantId = null;

        await CheckFeatures(tenancyName);

        if (!tenancyName.IsNullOrEmpty())
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var tenant = await TenantManager.FindByTenancyNameAsync(tenancyName);
                if (tenant == null)
                {
                    throw new UserFriendlyException(L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
                }

                tenantId = tenant.Id;
            }
        }

        using (CurrentUnitOfWork.SetTenantId(tenantId))
        {
            var user = await UserManager.FindByNameOrEmailAsync(userName);
            if (user == null)
            {
                throw new UserFriendlyException(L("ThereIsNoUserRegisteredWithNameOrEmail{0}", userName));
            }

            return user.ToUserIdentifier();
        }
    }

    private async Task CheckFeatures(string tenancyName)
    {
        if (ADTOSharpSession.TenantId == null)
        {
            return;
        }

        var tenantToTenantAllowed = await FeatureChecker.IsEnabledAsync
            ("App.ChatFeature.TenantToTenant");

        var tenantToHostAllowed = await FeatureChecker.IsEnabledAsync
            ("App.ChatFeature.TenantToHost");

        if (tenancyName.IsNullOrEmpty())
        {
            if (!tenantToHostAllowed)
            {
                throw new UserFriendlyException(L("TenantToHostChatIsNotEnabled"));
            }
        }
        else
        {
            if (!tenantToTenantAllowed)
            {
                throw new UserFriendlyException(L("TenantToTenantChatIsNotEnabled"));
            }
        }
    }

    #endregion

    #region Methods
    /// <summary>
    /// 创建好友申请
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<FriendDto> CreateFriendshipRequest(CreateFriendshipRequestInput input)
    {
        var userIdentifier = ADTOSharpSession.ToUserIdentifier();
        var probableFriend = new UserIdentifier(input.TenantId, input.UserId);

        _chatFeatureChecker.CheckChatFeatures(userIdentifier.TenantId, probableFriend.TenantId);

        if (await _friendshipManager.GetFriendshipOrNullAsync(userIdentifier, probableFriend) != null)
        {
            throw new UserFriendlyException(L("YouAlreadySentAFriendshipRequestToThisUser"));
        }

        var user = await UserManager.FindByIdAsync(ADTOSharpSession.GetUserId().ToString());

        User probableFriendUser;
        using (CurrentUnitOfWork.SetTenantId(input.TenantId))
        {
            probableFriendUser = await UserManager.FindByIdAsync(input.UserId.ToString());
        }

        // Friend requester
        var friendTenancyName = await GetTenancyNameAsync(probableFriend.TenantId);
        var sourceFriendship = new Friendship(userIdentifier, probableFriend, friendTenancyName,
            probableFriendUser.UserName, probableFriendUser.ProfilePictureId, FriendshipState.Accepted);
        await _friendshipManager.CreateFriendshipAsync(sourceFriendship);

        // Target friend
        var userTenancyName = await GetTenancyNameAsync(user.TenantId);
        var targetFriendship = new Friendship(probableFriend, userIdentifier, userTenancyName, user.UserName,
            user.ProfilePictureId, FriendshipState.Accepted);

        if (await _friendshipManager.GetFriendshipOrNullAsync(probableFriend, userIdentifier) == null)
        {
            await _friendshipManager.CreateFriendshipAsync(targetFriendship);

            var clients = await _onlineClientManager.GetAllByUserIdAsync(probableFriend);
            if (clients.Any())
            {
                var isFriendOnline = await _onlineClientManager.IsOnlineAsync(sourceFriendship.ToUserIdentifier());
                await _chatCommunicator.SendFriendshipRequestToClient(clients, targetFriendship, false,
                    isFriendOnline);
            }
        }

        var senderClients = await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
        if (senderClients.Any())
        {
            var isFriendOnline = await _onlineClientManager.IsOnlineAsync(targetFriendship.ToUserIdentifier());
            await _chatCommunicator.SendFriendshipRequestToClient(senderClients, sourceFriendship, true,
                isFriendOnline);
        }

        var sourceFriendshipRequest = ObjectMapper.Map<FriendDto>(sourceFriendship);
        sourceFriendshipRequest.IsOnline = (await _onlineClientManager.GetAllByUserIdAsync(probableFriend)).Any();

        return sourceFriendshipRequest;
    }
    /// <summary>
    /// 创建指定租户的好友申请
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<FriendDto> CreateFriendshipWithDifferentTenant(CreateFriendshipWithDifferentTenantInput input)
    {
        var probableFriend = await GetUserIdentifier(input.TenancyName, input.UserName);
        return await CreateFriendshipRequest(new CreateFriendshipRequestInput
        {
            TenantId = probableFriend.TenantId,
            UserId = probableFriend.UserId
        });
    }
    /// <summary>
    /// 创建当前租户的好友申请
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<FriendDto> CreateFriendshipForCurrentTenant(CreateFriendshipForCurrentTenantInput input)
    {
        using (CurrentUnitOfWork.SetTenantId(ADTOSharpSession.TenantId))
        {
            var user = await UserManager.FindByNameOrEmailAsync(input.UserName);
            if (user == null)
            {
                throw new UserFriendlyException(L("ThereIsNoUserRegisteredWithNameOrEmail{0}", input.UserName));
            }

            var probableFriend = user.ToUserIdentifier();

            return await CreateFriendshipRequest(new CreateFriendshipRequestInput
            {
                TenantId = probableFriend.TenantId,
                UserId = probableFriend.UserId
            });
        }

    }

    /// <summary>
    /// 拒绝好友申请
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task BlockUser(BlockUserInput input)
    {
        var userIdentifier = ADTOSharpSession.ToUserIdentifier();
        var friendIdentifier = new UserIdentifier(input.TenantId, input.UserId);
        await _friendshipManager.BanFriendAsync(userIdentifier, friendIdentifier);

        var clients = await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
        if (clients.Any())
        {
            await _chatCommunicator.SendUserStateChangeToClients(clients, friendIdentifier,
                FriendshipState.Blocked);
        }
    }
    /// <summary>
    /// 接受好友申请
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UnblockUser(UnblockUserInput input)
    {
        var userIdentifier = ADTOSharpSession.ToUserIdentifier();
        var friendIdentifier = new UserIdentifier(input.TenantId, input.UserId);
        await _friendshipManager.AcceptFriendshipRequestAsync(userIdentifier, friendIdentifier);

        var clients =  await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
        if (clients.Any())
        {
            await _chatCommunicator.SendUserStateChangeToClients(clients, friendIdentifier,
                FriendshipState.Accepted);
        }
    }
    /// <summary>
    /// 接受好友申请
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task AcceptFriendshipRequest(AcceptFriendshipRequestInput input)
    {
        var userIdentifier = ADTOSharpSession.ToUserIdentifier();
        var friendIdentifier = new UserIdentifier(input.TenantId, input.UserId);
        await _friendshipManager.AcceptFriendshipRequestAsync(userIdentifier, friendIdentifier);

        var clients = await _onlineClientManager.GetAllByUserIdAsync(userIdentifier);
        if (clients.Any())
        {
            await _chatCommunicator.SendUserStateChangeToClients(clients, friendIdentifier,
                FriendshipState.Accepted);
        }
    }

    /// <summary>
    /// 移除好友
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task RemoveFriend(RemoveFriendInput input)
    {
        var userIdentifier = ADTOSharpSession.ToUserIdentifier();
        var friendIdentifier = new UserIdentifier(input.TenantId, input.UserId);
        await _friendshipManager.RemoveFriendAsync(userIdentifier, friendIdentifier);
    }


    [HttpPost]
    public async Task<ListResultDto<FriendDto>> GetAllFriendshipsAsync()
    {
        var userIdentifier = ADTOSharpSession.ToUserIdentifier();
        var clients =  await _friendshipManager.GetAllFriendshipsAsync(userIdentifier);
        return new ListResultDto<FriendDto>(ObjectMapper.Map<List<FriendDto>>(clients));
    }


    #endregion
}
