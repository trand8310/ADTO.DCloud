using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization;
using ADTOSharp.BackgroundJobs;
using ADTOSharp.Configuration;
using ADTOSharp.Extensions;
using ADTOSharp.Localization;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Timing;
using ADTOSharp.UI;
using ADTOSharp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Authorization.Users.Profile.Cache;
using ADTO.DCloud.Authorization.Users.Profile.Dto;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.Friendships;
using ADTO.DCloud.Net.Sms;
using ADTO.DCloud.Security;
using ADTO.DCloud.Storage;
using ADTO.DCloud.Timing;
using ADTO.DCloud.Gdpr;

namespace ADTO.DCloud.Authorization.Users.Profile;

/// <summary>
/// 用户偏好配置
/// </summary>
[ADTOSharpAuthorize]
public class ProfileAppService : DCloudAppServiceBase, IProfileAppService
{
    #region Fields
    private const int MaxProfilePictureBytes = 5242880; //5MB
    private readonly IBinaryObjectManager _binaryObjectManager;
    private readonly ITimeZoneService _timeZoneService;
    private readonly IFriendshipManager _friendshipManager;
    private readonly ISmsSender _smsSender;
    private readonly ICacheManager _cacheManager;
    private readonly ITempFileCacheManager _tempFileCacheManager;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly ProfileImageServiceFactory _profileImageServiceFactory;
    private readonly ISettingStore _settingStore;
    private readonly ITypedCache<string, Dictionary<string, SettingInfo>> _userSettingCache;
    #endregion
    #region Ctor
    public ProfileAppService(
        IBinaryObjectManager binaryObjectManager,
        ITimeZoneService timezoneService,
        IFriendshipManager friendshipManager,
        ISmsSender smsSender,
        ICacheManager cacheManager,
        ITempFileCacheManager tempFileCacheManager,
        IBackgroundJobManager backgroundJobManager,
        ProfileImageServiceFactory profileImageServiceFactory,
        ISettingStore settingStore)
    {
        _binaryObjectManager = binaryObjectManager;
        _timeZoneService = timezoneService;
        _friendshipManager = friendshipManager;
        _smsSender = smsSender;
        _cacheManager = cacheManager;
        _tempFileCacheManager = tempFileCacheManager;
        _backgroundJobManager = backgroundJobManager;
        _profileImageServiceFactory = profileImageServiceFactory;
        _settingStore = settingStore;
        _userSettingCache = cacheManager.GetUserSettingsCache();
    }
    #endregion

    #region Methods
    /// <summary>
    /// 编辑当前用户信息
    /// </summary>
    /// <returns></returns>
    [DisableAuditing]
    public async Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit()
    {
        var user = await GetCurrentUserAsync();
        var userProfileEditDto = ObjectMapper.Map<CurrentUserProfileEditDto>(user);

        if (!Clock.SupportsMultipleTimezone)
        {
            return userProfileEditDto;
        }

        userProfileEditDto.Timezone = await SettingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);

        var defaultTimeZoneId = await _timeZoneService.GetDefaultTimezoneAsync(
            SettingScopes.User,
            ADTOSharpSession.TenantId
        );

        if (userProfileEditDto.Timezone == defaultTimeZoneId)
        {
            userProfileEditDto.Timezone = string.Empty;
        }

        return userProfileEditDto;
    }
    /// <summary>
    /// 发送短信验证码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task SendVerificationSms(SendVerificationSmsInputDto input)
    {
        var code = RandomHelper.GetRandom(100000, 999999).ToString();
        var cacheKey = ADTOSharpSession.ToUserIdentifier().ToString();
        var cacheItem = new SmsVerificationCodeCacheItem
        {
            Code = code
        };

        await _cacheManager.GetSmsVerificationCodeCache().SetAsync(
            cacheKey,
            cacheItem
        );

        await _smsSender.SendAsync(input.PhoneNumber, L("SmsVerificationMessage", code));
    }

    /// <summary>
    /// 校验短信验证码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task VerifySmsCode(VerifySmsCodeInputDto input)
    {
        var cacheKey = ADTOSharpSession.ToUserIdentifier().ToString();
        var cash = await _cacheManager.GetSmsVerificationCodeCache().GetOrDefaultAsync(cacheKey);

        if (cash == null)
        {
            throw new Exception("Phone number confirmation code is not found in cache !");
        }

        if (input.Code != cash.Code)
        {
            throw new UserFriendlyException(L("WrongSmsVerificationCode"));
        }

        var user = await UserManager.GetUserAsync(ADTOSharpSession.ToUserIdentifier());
        user.IsPhoneNumberConfirmed = true;
        user.PhoneNumber = input.PhoneNumber;
        await UserManager.UpdateAsync(user);
    }

    public async Task PrepareCollectedData()
    {
        await _backgroundJobManager.EnqueueAsync<UserCollectedDataPrepareJob, UserIdentifier>(
            ADTOSharpSession.ToUserIdentifier()
        );
    }

    public async Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input)
    {
        var user = await GetCurrentUserAsync();

        if (user.PhoneNumber != input.PhoneNumber)
        {
            input.IsPhoneNumberConfirmed = false;
        }
        else if (user.IsPhoneNumberConfirmed)
        {
            input.IsPhoneNumberConfirmed = true;
        }

        ObjectMapper.Map(input, user);
        CheckErrors(await UserManager.UpdateAsync(user));

        if (Clock.SupportsMultipleTimezone)
        {
            if (input.Timezone.IsNullOrEmpty())
            {
                var defaultValue =
                    await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, ADTOSharpSession.TenantId);
                await SettingManager.ChangeSettingForUserAsync(ADTOSharpSession.ToUserIdentifier(),
                    TimingSettingNames.TimeZone, defaultValue);
            }
            else
            {
                await SettingManager.ChangeSettingForUserAsync(ADTOSharpSession.ToUserIdentifier(),
                    TimingSettingNames.TimeZone, input.Timezone);
            }
        }
    }

    public async Task ChangePassword(ChangePasswordInput input)
    {
        await UserManager.InitializeOptionsAsync(ADTOSharpSession.TenantId);

        var user = await GetCurrentUserAsync();
        if (await UserManager.CheckPasswordAsync(user, input.CurrentPassword))
        {
            CheckErrors(await UserManager.ChangePasswordAsync(user, input.NewPassword));
        }
        else
        {
            CheckErrors(IdentityResult.Failed(new IdentityError
            {
                Description = "Incorrect password."
            }));
        }
    }

    /// <summary>
    /// 修改用户图像
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateProfilePicture(UpdateProfilePictureInput input)
    {
        var userId = ADTOSharpSession.GetUserId();
        if (input.UserId.HasValue && input.UserId.Value != userId)
        {
            await CheckUpdateUsersProfilePicturePermission();
            userId = input.UserId.Value;
        }

        await UpdateProfilePictureForUser(userId, input);
    }

    /// <summary>
    /// 修改用户图像，并返回用户图像
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<string> UpdateProfilePictureReturnPicture(UpdateProfilePictureInput input)
     {
        var userId = ADTOSharpSession.GetUserId();
        if (input.UserId.HasValue && input.UserId.Value != userId)
        {
            await CheckUpdateUsersProfilePicturePermission();
            userId = input.UserId.Value;
        }

        await UpdateProfilePictureForUser(userId, input);

        await CurrentUnitOfWork.SaveChangesAsync();
        //把用户图像返回
        var imageUser = await GetProfilePictureByUser(userId);
        return imageUser.ProfilePicture;
    }


    private async Task CheckUpdateUsersProfilePicturePermission()
    {
        var permissionToChangeAnotherUsersProfilePicture = await PermissionChecker.IsGrantedAsync(
            PermissionNames.Pages_Administration_Users_ChangeProfilePicture
        );

        if (!permissionToChangeAnotherUsersProfilePicture)
        {
            var localizedPermissionName = L("UpdateUsersProfilePicture");
            throw new ADTOSharpAuthorizationException(
                string.Format(
                    L("AllOfThesePermissionsMustBeGranted"),
                    localizedPermissionName
                )
            );
        }
    }

    /// <summary>
    /// 更新用户图像
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    private async Task UpdateProfilePictureForUser(Guid userId, UpdateProfilePictureInput input)
    {
        var userIdentifier = new UserIdentifier(ADTOSharpSession.TenantId, userId);
        var allowToUseGravatar = await SettingManager.GetSettingValueForUserAsync<bool>(
            AppSettings.UserManagement.AllowUsingGravatarProfilePicture,
            user: userIdentifier
        );

        if (!allowToUseGravatar)
        {
            input.UseGravatarProfilePicture = false;
        }

        await SettingManager.ChangeSettingForUserAsync(
            userIdentifier,
            AppSettings.UserManagement.UseGravatarProfilePicture,
            input.UseGravatarProfilePicture.ToString().ToLowerInvariant()
        );

        if (input.UseGravatarProfilePicture)
        {
            return;
        }

        byte[] byteArray;

        var imageBytes = _tempFileCacheManager.GetFile(input.FileToken);

        if (imageBytes == null)
        {
            throw new UserFriendlyException("There is no such image file with the token: " + input.FileToken);
        }

        byteArray = imageBytes;

        if (byteArray.Length > MaxProfilePictureBytes)
        {
            throw new UserFriendlyException(L("ResizedProfilePicture_Warn_SizeLimit",
                AppConsts.ResizedMaxProfilePictureBytesUserFriendlyValue));
        }

        var user = await UserManager.GetUserByIdAsync(userIdentifier.UserId);

        if (user.ProfilePictureId.HasValue)
        {
            await _binaryObjectManager.DeleteAsync(user.ProfilePictureId.Value);
        }

        var storedFile = new BinaryObject(userIdentifier.TenantId, byteArray,
            $"Profile picture of user {userIdentifier.UserId}. {DateTime.UtcNow}");
        await _binaryObjectManager.SaveAsync(storedFile);

        user.ProfilePictureId = storedFile.Id;
    }

    /// <summary>
    /// 获取密码复杂度设置
    /// </summary>
    /// <returns></returns>
    [ADTOSharpAllowAnonymous]
    public async Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting()
    {
        var passwordComplexitySetting = new PasswordComplexitySetting
        {
            RequireDigit =
                await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                    .PasswordComplexity.RequireDigit),
            RequireLowercase =
                await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                    .PasswordComplexity.RequireLowercase),
            RequireNonAlphanumeric =
                await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                    .PasswordComplexity.RequireNonAlphanumeric),
            RequireUppercase =
                await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                    .PasswordComplexity.RequireUppercase),
            RequiredLength =
                await SettingManager.GetSettingValueAsync<int>(ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity
                    .RequiredLength)
        };

        return new GetPasswordComplexitySettingOutput
        {
            Setting = passwordComplexitySetting
        };
    }
    /// <summary>
    /// 获取用户图像
    /// </summary>
    /// <returns></returns>
    [DisableAuditing]
    public async Task<GetProfilePictureOutput> GetProfilePicture()
    {
        using (var profileImageService = await _profileImageServiceFactory.Get(ADTOSharpSession.ToUserIdentifier()))
        {
            var profilePictureContent = await profileImageService.Object.GetProfilePictureContentForUser(
                ADTOSharpSession.ToUserIdentifier()
            );

            return new GetProfilePictureOutput(profilePictureContent);
        }
    }

    /// <summary>
    /// 获取指定用户的图像
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    [ADTOSharpAllowAnonymous]
    public async Task<GetProfilePictureOutput> GetProfilePictureByUserName(string username)
    {
        var user = await UserManager.FindByNameAsync(username);
        if (user == null)
        {
            return new GetProfilePictureOutput(string.Empty);
        }

        var userIdentifier = new UserIdentifier(ADTOSharpSession.TenantId, user.Id);
        using (var profileImageService = await _profileImageServiceFactory.Get(userIdentifier))
        {
            var profileImage = await profileImageService.Object.GetProfilePictureContentForUser(userIdentifier);
            return new GetProfilePictureOutput(profileImage);
        }
    }

    /// <summary>
    /// 获取好友图像
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<GetProfilePictureOutput> GetFriendProfilePicture(GetFriendProfilePictureInput input)
    {
        var friendUserIdentifier = input.ToUserIdentifier();
        var friendShip = await _friendshipManager.GetFriendshipOrNullAsync(
            ADTOSharpSession.ToUserIdentifier(),
            friendUserIdentifier
        );

        if (friendShip == null)
        {
            return new GetProfilePictureOutput(string.Empty);
        }


        using (var profileImageService = await _profileImageServiceFactory.Get(friendUserIdentifier))
        {
            var image = await profileImageService.Object.GetProfilePictureContentForUser(friendUserIdentifier);
            return new GetProfilePictureOutput(image);
        }
    }

    /// <summary>
    /// 获取指定用户Id的用户图像
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [ADTOSharpAllowAnonymous]
    public async Task<GetProfilePictureOutput> GetProfilePictureByUser(Guid userId)
    {
        var userIdentifier = new UserIdentifier(ADTOSharpSession.TenantId, userId);
        using (var profileImageService = await _profileImageServiceFactory.Get(userIdentifier))
        {
            var profileImage = await profileImageService.Object.GetProfilePictureContentForUser(userIdentifier);
            return new GetProfilePictureOutput(profileImage);
        }
    }
    /// <summary>
    /// 变更当前用户的语言设置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    public async Task ChangeLanguage(ChangeUserLanguageDto input)
    {
        var languageSetting = await _settingStore.GetSettingOrNullAsync(
            ADTOSharpSession.TenantId,
            ADTOSharpSession.GetUserId(),
            LocalizationSettingNames.DefaultLanguage
        );

        if (languageSetting == null)
        {
            await _settingStore.CreateAsync(new SettingInfo(
                ADTOSharpSession.TenantId,
                ADTOSharpSession.UserId,
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            ));
        }
        else
        {
            await _settingStore.UpdateAsync(new SettingInfo(
                ADTOSharpSession.TenantId,
                ADTOSharpSession.UserId,
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            ));
        }

        await _userSettingCache.RemoveAsync(ADTOSharpSession.ToUserIdentifier().ToString());
    }

    private async Task<byte[]> GetProfilePictureByIdOrNull(Guid profilePictureId)
    {
        var file = await _binaryObjectManager.GetOrNullAsync(profilePictureId);
        if (file == null)
        {
            return null;
        }

        return file.Bytes;
    }

    private async Task<GetProfilePictureOutput> GetProfilePictureByIdInternal(Guid profilePictureId)
    {
        var bytes = await GetProfilePictureByIdOrNull(profilePictureId);
        if (bytes == null)
        {
            return new GetProfilePictureOutput(string.Empty);
        }

        return new GetProfilePictureOutput(Convert.ToBase64String(bytes));
    }
    #endregion
}
