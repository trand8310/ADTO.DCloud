using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Extensions;
using ADTOSharp.Runtime.Session;
using Hangfire.Dashboard;

namespace ADTOSharp.Hangfire
{
    public class ADTOSharpHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public IIocResolver IocResolver { get; set; }

        private readonly string _requiredPermissionName;

        public ADTOSharpHangfireAuthorizationFilter(string requiredPermissionName = null)
        {
            _requiredPermissionName = requiredPermissionName;

            IocResolver = IocManager.Instance;
        }

        public bool Authorize(DashboardContext context)
        {
            if (!IsLoggedIn())
            {
                return false;
            }

            if (!_requiredPermissionName.IsNullOrEmpty() && !IsPermissionGranted(_requiredPermissionName))
            {
                return false;
            }

            return true;
        }

        private bool IsLoggedIn()
        {
            using (var session = IocResolver.ResolveAsDisposable<IADTOSharpSession>())
            {
                return session.Object.UserId.HasValue;
            }
        }

        private bool IsPermissionGranted(string requiredPermissionName)
        {
            using (var permissionChecker = IocResolver.ResolveAsDisposable<IPermissionChecker>())
            {
                return permissionChecker.Object.IsGranted(requiredPermissionName);
            }
        }
    }
}
