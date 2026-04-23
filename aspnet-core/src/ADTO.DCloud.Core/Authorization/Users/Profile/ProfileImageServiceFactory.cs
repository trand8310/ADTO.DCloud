using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTO.DCloud.Configuration;

namespace ADTO.DCloud.Authorization.Users.Profile
{
    public class ProfileImageServiceFactory : ITransientDependency
    {
        private readonly ISettingManager _settingManager;
        private readonly IIocResolver _iocResolver;

        public ProfileImageServiceFactory(
            ISettingManager settingManager,
            IIocResolver iocResolver)
        {
            _settingManager = settingManager;
            _iocResolver = iocResolver;
        }

        public async Task<IDisposableDependencyObjectWrapper<IProfileImageService>> Get(UserIdentifier userIdentifier)
        {
            if (await _settingManager.GetSettingValueForUserAsync<bool>(AppSettings.UserManagement.UseGravatarProfilePicture, userIdentifier))
            {
                return _iocResolver.ResolveAsDisposable<GravatarProfileImageService>();
            }

            return _iocResolver.ResolveAsDisposable<LocalProfileImageService>();
        }
    }
}