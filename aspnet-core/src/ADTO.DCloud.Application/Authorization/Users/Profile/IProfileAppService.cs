using System;
using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Authorization.Users.Profile.Dto;
using ADTO.DCloud.Dto;

namespace ADTO.DCloud.Authorization.Users.Profile
{
    public interface IProfileAppService : IApplicationService
    {
        Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit();

        Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input);

        Task ChangePassword(ChangePasswordInput input);

        Task UpdateProfilePicture(UpdateProfilePictureInput input);

        Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting();

        Task<GetProfilePictureOutput> GetProfilePicture();

        Task<GetProfilePictureOutput> GetProfilePictureByUser(Guid userId);
        
        Task<GetProfilePictureOutput> GetProfilePictureByUserName(string username);

        Task<GetProfilePictureOutput> GetFriendProfilePicture(GetFriendProfilePictureInput input);

        Task ChangeLanguage(ChangeUserLanguageDto input);
        
        Task SendVerificationSms(SendVerificationSmsInputDto input);

        Task VerifySmsCode(VerifySmsCodeInputDto input);

        Task PrepareCollectedData();
        
        //Task<GenerateGoogleAuthenticatorKeyOutput> GenerateGoogleAuthenticatorKey();
        
        //Task<UpdateGoogleAuthenticatorKeyOutput> UpdateGoogleAuthenticatorKey(UpdateGoogleAuthenticatorKeyInput input);
        
        //Task<bool> VerifyAuthenticatorCode(VerifyAuthenticatorCodeInput input);
        
        //Task DisableGoogleAuthenticator(VerifyAuthenticatorCodeInput input);
        
       // Task<UpdateGoogleAuthenticatorKeyOutput> ViewRecoveryCodes(VerifyAuthenticatorCodeInput input);
        
    }
}
