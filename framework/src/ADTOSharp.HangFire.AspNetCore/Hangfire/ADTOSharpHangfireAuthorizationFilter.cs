using ADTOSharp.Authorization;
using ADTOSharp.Extensions;
using ADTOSharp.Runtime.Session;
using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace ADTOSharp.Hangfire
{
    public class ADTOSharpHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string _requiredPermissionName;

        public ADTOSharpHangfireAuthorizationFilter(string requiredPermissionName = null)
        {
            _requiredPermissionName = requiredPermissionName;
        }

        public bool Authorize(DashboardContext context)
        {
            if (!IsLoggedIn(context))
            {
                return false;
            }

            if (!_requiredPermissionName.IsNullOrEmpty() && !IsPermissionGranted(context, _requiredPermissionName))
            {
                return false;
            }

            return true;
        }

        private static bool IsLoggedIn(DashboardContext context)
        {
            var session = context.GetHttpContext().RequestServices.GetRequiredService<IADTOSharpSession>();
            return session.UserId.HasValue;
        }

        private static bool IsPermissionGranted(DashboardContext context, string requiredPermissionName)
        {
            var permissionChecker = context.GetHttpContext().RequestServices.GetRequiredService<IPermissionChecker>();
            return permissionChecker.IsGranted(requiredPermissionName);
        }
    }
}
