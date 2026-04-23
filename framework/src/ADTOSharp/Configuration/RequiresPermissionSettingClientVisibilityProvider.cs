using System.Threading.Tasks;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Session;

namespace ADTOSharp.Configuration
{
    public class RequiresPermissionSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        private readonly IPermissionDependency _permissionDependency;

        public RequiresPermissionSettingClientVisibilityProvider(IPermissionDependency permissionDependency)
        {
            _permissionDependency = permissionDependency;
        }

        public async Task<bool> CheckVisible(IScopedIocResolver scope)
        {
            var adtoSession = scope.Resolve<IADTOSharpSession>();

            if (!adtoSession.UserId.HasValue)
            {
                return false;
            }

            var permissionDependencyContext = scope.Resolve<PermissionDependencyContext>();
            permissionDependencyContext.User = adtoSession.ToUserIdentifier();

            return await _permissionDependency.IsSatisfiedAsync(permissionDependencyContext);
        }
    }
}