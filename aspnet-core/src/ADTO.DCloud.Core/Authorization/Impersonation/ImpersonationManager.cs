using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Security;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using ADTO.DCloud.Authorization.Users;

namespace ADTO.DCloud.Authorization.Impersonation;
/// <summary>
/// 模拟帐号管理
/// </summary>
public class ImpersonationManager : DCloudDomainServiceBase, IImpersonationManager
{
    public IADTOSharpSession ADTOSharpSession { get; set; }

    private readonly ICacheManager _cacheManager;
    private readonly UserManager _userManager;
    private readonly UserClaimsPrincipalFactory _principalFactory;

    public ImpersonationManager(
        ICacheManager cacheManager,
        UserManager userManager,
        UserClaimsPrincipalFactory principalFactory)
    {
        _cacheManager = cacheManager;
        _userManager = userManager;
        _principalFactory = principalFactory;

        ADTOSharpSession = NullADTOSharpSession.Instance;
    }
    /// <summary>
    /// 用户模拟帐号用户标识
    /// </summary>
    /// <param name="impersonationToken"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<UserAndIdentity> GetImpersonatedUserAndIdentity(string impersonationToken)
    {
        var cacheItem = await _cacheManager.GetImpersonationCache().GetOrDefaultAsync(impersonationToken);
        if (cacheItem == null)
        {
            throw new UserFriendlyException(L("ImpersonationTokenErrorMessage"));
        }

        CheckCurrentTenant(cacheItem.TargetTenantId);

        //Get the user from tenant
        var user = await _userManager.FindByIdAsync(cacheItem.TargetUserId.ToString());

        //Create identity
        var identity = await GetClaimsIdentityFromCache(user, cacheItem);

        //Remove the cache item to prevent re-use
        await _cacheManager.GetImpersonationCache().RemoveAsync(impersonationToken);

        return new UserAndIdentity(user, identity);
    }

    /// <summary>
    /// 在模拟帐号环境下,得到模拟帐号的用户声明
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cacheItem"></param>
    /// <returns></returns>
    private async Task<ClaimsIdentity> GetClaimsIdentityFromCache(User user, ImpersonationCacheItem cacheItem)
    {
        var identity = (ClaimsIdentity) (await _principalFactory.CreateAsync(user)).Identity;

        if (!cacheItem.IsBackToImpersonator)
        {
            //Add claims for audit logging
            if (cacheItem.ImpersonatorTenantId.HasValue)
            {
                identity.AddClaim(new Claim(ADTOSharpClaimTypes.ImpersonatorTenantId,
                    cacheItem.ImpersonatorTenantId.Value.ToString("n")));
            }

            identity.AddClaim(new Claim(ADTOSharpClaimTypes.ImpersonatorUserId,
                cacheItem.ImpersonatorUserId.ToString("n")));
        }

        return identity;
    }
    /// <summary>
    /// 获取模拟帐号的TOKEN
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public Task<string> GetImpersonationToken(Guid userId, Guid? tenantId)
    {
        if (ADTOSharpSession.ImpersonatorUserId.HasValue)
        {
            throw new UserFriendlyException(L("CascadeImpersonationErrorMessage"));
        }

        if (ADTOSharpSession.TenantId.HasValue)
        {
            if (!tenantId.HasValue)
            {
                throw new UserFriendlyException(L("FromTenantToHostImpersonationErrorMessage"));
            }

            if (tenantId.Value != ADTOSharpSession.TenantId.Value)
            {
                throw new UserFriendlyException(L("DifferentTenantImpersonationErrorMessage"));
            }
        }

        return GenerateImpersonationTokenAsync(tenantId, userId, false);
    }
    /// <summary>
    /// 在退出模拟帐号时 ,获取发起模拟前的TOKEN
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>

    public Task<string> GetBackToImpersonatorToken()
    {
        if (!ADTOSharpSession.ImpersonatorUserId.HasValue)
        {
            throw new UserFriendlyException(L("NotImpersonatedLoginErrorMessage"));
        }

        return GenerateImpersonationTokenAsync(ADTOSharpSession.ImpersonatorTenantId, ADTOSharpSession.ImpersonatorUserId.Value, true);
    }


    private void CheckCurrentTenant(Guid? tenantId)
    {
        if (ADTOSharpSession.TenantId != tenantId)
        {
            throw new Exception($"Current tenant is different than given tenant. ADTOSharpSession.TenantId: {ADTOSharpSession.TenantId}, given tenantId: {tenantId}");
        }
    }
    /// <summary>
    /// 生成模拟帐号TOKEN
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="userId"></param>
    /// <param name="isBackToImpersonator"></param>
    /// <returns></returns>
    private async Task<string> GenerateImpersonationTokenAsync(Guid? tenantId, Guid userId, bool isBackToImpersonator)
    {
        //Create a cache item
        var cacheItem = new ImpersonationCacheItem(
            tenantId,
            userId,
            isBackToImpersonator
        );

        if (!isBackToImpersonator)
        {
            cacheItem.ImpersonatorTenantId = ADTOSharpSession.TenantId;
            cacheItem.ImpersonatorUserId = ADTOSharpSession.GetUserId();
        }

        //Create a random token and save to the cache
        var token = Guid.NewGuid().ToString();

        await _cacheManager
            .GetImpersonationCache()
            .SetAsync(token, cacheItem, TimeSpan.FromMinutes(1));

        return token;
    }
}

