using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTO.DCloud.Authentication.PasswordlessLogin;
using ADTO.DCloud.Authorization.Users;
using ADTOSharp;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ADTO.DCloud.Authorization.PasswordlessLogin;

public class PasswordlessLoginManager : DCloudDomainServiceBase, IPasswordlessLoginManager
{
    #region Fields
    private readonly IUserRepository _userRepository;
    private readonly UserManager _userManager;
    private readonly ICacheManager _cacheManager;
    #endregion

    #region Ctor
    public PasswordlessLoginManager(
        IUserRepository userRepository,
        UserManager userManager,
        ICacheManager cacheManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _cacheManager = cacheManager;
    }
    #endregion

    #region Methods
    /// <summary>
    /// 使用第三方唯一的KEY,获取系统中存在映射关系的用户
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="providerKey"></param>
    /// <returns></returns>
    /// <exception cref="ADTOSharpException"></exception>
    public async Task<User> GetUserByPasswordlessProviderAndKeyAsync(string provider, string providerKey)
    {
        if (provider == PasswordlessLoginProviderType.Email.ToString())
        {
            return await _userManager.FindByEmailAsync(providerKey);
        }

        if (provider == PasswordlessLoginProviderType.Sms.ToString())
        {
            return await _userRepository.FindByPhoneNumberAsync(providerKey);
        }
        if (provider == PasswordlessLoginProviderType.QRCODE.ToString())
        {
            return await _userRepository.GetAsync(Guid.Parse(providerKey));
        }

        throw new ADTOSharpException(L("UserNotFound"));
    }

    /// <summary>
    /// 校验提供的CODE是否与缓存中的一至,相当于密码校验
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="providerValue"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task VerifyPasswordlessLoginCode(Guid? tenantId, string providerValue, string code)
    {
        var cacheKey = GetPasswordlessLoginCodeCacheKey(tenantId, providerValue);
        var cache = await _cacheManager.GetPasswordlessVerificationCodeCache().GetOrDefaultAsync(cacheKey);

        if (cache == null)
        {
            throw new Exception(L("PasswordlessCodeNotFoundCache"));
        }

        if (code != cache.Code)
        {
            throw new UserFriendlyException(L("WrongPasswordlessVerificationCode"));
        }
    }
    /// <summary>
    /// 生成随机的CODE
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="providerKey"></param>
    /// <returns></returns>
    public async Task<string> GeneratePasswordlessLoginCode(Guid? tenantId, string providerKey)
    {
        var code = Guid.NewGuid().ToString("N");// RandomHelper.GetRandom(100000, 999999).ToString();
        var cacheItem = new PasswordlessLoginCodeCacheItem
        {
            Code = code
        };

        // 删除旧的Code， 保证一次只有一个Code生效
        await _cacheManager.GetPasswordlessVerificationCodeCache()
            .RemoveAsync(
                GetPasswordlessLoginCodeCacheKey(tenantId, providerKey)
            );

        await _cacheManager.GetPasswordlessVerificationCodeCache().SetAsync(
            GetPasswordlessLoginCodeCacheKey(tenantId, providerKey),
            cacheItem
        );

        return code;
    }

    /// <summary>
    /// 删除指定providerKey的code
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="providerKey"></param>
    /// <returns></returns>
    public async Task RemovePasswordlessLoginCode(Guid? tenantId, string providerKey)
    {
        await _cacheManager.GetPasswordlessVerificationCodeCache().RemoveAsync(
            GetPasswordlessLoginCodeCacheKey(tenantId, providerKey)
        );
    }

    /// <summary>
    /// 获取所有免密码登录的第三方提供者,如EMAIL,SMS,QRCODE
    /// </summary>
    /// <returns></returns>
    
    public List<SelectListItem> GetProviders()
    {
        return Enum.GetValues(typeof(PasswordlessLoginProviderType))
            .Cast<PasswordlessLoginProviderType>()
            .Select(providerType => new SelectListItem
            {
                Text = Enum.GetName(typeof(PasswordlessLoginProviderType), providerType),
                Value = providerType.ToString("G")
            })
            .ToList();
    }

    private string GetPasswordlessLoginCodeCacheKey(Guid? tenantId, string providerKey)
    {
        if (tenantId.HasValue)
        {
            return tenantId.Value + "|" + providerKey;
        }

        return providerKey;
    }
    #endregion
}
