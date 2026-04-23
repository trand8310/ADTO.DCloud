using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Security;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using ADTO.DCloud.Authorization.Impersonation;

namespace ADTO.DCloud.Authorization.Users;

public class UserLinkManager : DCloudDomainServiceBase, IUserLinkManager
{
    private readonly IRepository<UserAccount, Guid> _userAccountRepository;
    private readonly ICacheManager _cacheManager;
    private readonly UserManager _userManager;
    private readonly UserClaimsPrincipalFactory _principalFactory;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    
    public IADTOSharpSession ADTOSharpSession { get; set; }

    public UserLinkManager(
        IRepository<UserAccount, Guid> userAccountRepository,
        ICacheManager cacheManager, 
        UserManager userManager,
        UserClaimsPrincipalFactory principalFactory, 
        IUnitOfWorkManager unitOfWorkManager)
    {
        _userAccountRepository = userAccountRepository;
        _cacheManager = cacheManager;
        _userManager = userManager;
        _principalFactory = principalFactory;
        _unitOfWorkManager = unitOfWorkManager;
        ADTOSharpSession = NullADTOSharpSession.Instance;
    }
    
    public virtual async Task Link(User firstUser, User secondUser)
    {
        await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            var firstUserAccount = await GetUserAccountAsync(firstUser.ToUserIdentifier());
            var secondUserAccount = await GetUserAccountAsync(secondUser.ToUserIdentifier());

            var userLinkId = firstUserAccount.UserLinkId ?? firstUserAccount.Id;
            firstUserAccount.UserLinkId = userLinkId;

            var userAccountsToLink = secondUserAccount.UserLinkId.HasValue
                ? await _userAccountRepository.GetAllListAsync(ua => ua.UserLinkId == secondUserAccount.UserLinkId.Value)
                : new List<UserAccount> { secondUserAccount };

            userAccountsToLink.ForEach(u =>
            {
                u.UserLinkId = userLinkId;
            });

            await CurrentUnitOfWork.SaveChangesAsync();
        });
    }

    public virtual async Task<bool> AreUsersLinked(UserIdentifier firstUserIdentifier, UserIdentifier secondUserIdentifier)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            var firstUserAccount = await GetUserAccountAsync(firstUserIdentifier);
            var secondUserAccount = await GetUserAccountAsync(secondUserIdentifier);

            if (!firstUserAccount.UserLinkId.HasValue || !secondUserAccount.UserLinkId.HasValue)
            {
                return false;
            }

            return firstUserAccount.UserLinkId == secondUserAccount.UserLinkId;
        });
    }
    
    public virtual async Task Unlink(UserIdentifier userIdentifier)
    {
        await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            var targetUserAccount = await GetUserAccountAsync(userIdentifier);
            targetUserAccount.UserLinkId = null;

            await CurrentUnitOfWork.SaveChangesAsync();
        });
    }
    
    public virtual async Task<UserAccount> GetUserAccountAsync(UserIdentifier userIdentifier)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await _userAccountRepository.FirstOrDefaultAsync(
                ua => ua.TenantId == userIdentifier.TenantId && ua.UserId == userIdentifier.UserId
            );
        });
    }

    public async Task<string> GetAccountSwitchToken(Guid targetUserId, Guid? targetTenantId)
    {
        //Create a cache item
        var cacheItem = new SwitchToLinkedAccountCacheItem(
            targetTenantId,
            targetUserId,
            ADTOSharpSession.ImpersonatorTenantId,
            ADTOSharpSession.ImpersonatorUserId
        );

        //Create a random token and save to the cache
        var token = Guid.NewGuid().ToString();

        await _cacheManager
            .GetSwitchToLinkedAccountCache()
            .SetAsync(token, cacheItem, TimeSpan.FromMinutes(1));

        return token;
    }

    public async Task<UserAndIdentity> GetSwitchedUserAndIdentity(string switchAccountToken)
    {
        var cacheItem = await _cacheManager.GetSwitchToLinkedAccountCache().GetOrDefaultAsync(switchAccountToken);
        if (cacheItem == null)
        {
            throw new UserFriendlyException(L("SwitchToLinkedAccountTokenErrorMessage"));
        }

        //Get the user from tenant
        var user = await _userManager.FindByIdAsync(cacheItem.TargetUserId.ToString());

        //Create identity
        var identity = (ClaimsIdentity)(await _principalFactory.CreateAsync(user)).Identity;

        //Add claims for audit logging
        if (cacheItem.ImpersonatorTenantId.HasValue)
        {
            identity.AddClaim(new Claim(ADTOSharpClaimTypes.ImpersonatorTenantId, cacheItem.ImpersonatorTenantId.Value.ToString("N")));
        }

        if (cacheItem.ImpersonatorUserId.HasValue)
        {
            identity.AddClaim(new Claim(ADTOSharpClaimTypes.ImpersonatorUserId, cacheItem.ImpersonatorUserId.Value.ToString("N")));
        }

        //Remove the cache item to prevent re-use
        await _cacheManager.GetSwitchToLinkedAccountCache().RemoveAsync(switchAccountToken);

        return new UserAndIdentity(user, identity);
    }
}

