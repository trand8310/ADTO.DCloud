using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Configuration;
using ADTOSharp.Domain.Uow;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Runtime.Session;

namespace ADTO.DCloud.Identity
{
    public class SignInManager : ADTOSharpSignInManager<Tenant, Role, User>
    {
        private readonly ISettingManager _settingManager;
        private readonly IADTOSharpSession _session;


        public SignInManager(
            UserManager userManager,
            IHttpContextAccessor contextAccessor,
            UserClaimsPrincipalFactory claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<User> userConfirmation,
            IADTOSharpSession session)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, unitOfWorkManager, settingManager, schemes, userConfirmation)
        {
            _settingManager = settingManager;
            _session = session;
        }

        public override async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            var schemes = await base.GetExternalAuthenticationSchemesAsync();

            if (_session.TenantId.HasValue)
            {
                schemes = schemes.Where(IsSchemeEnabledOnTenant);
            }

            return schemes;
        }

        private bool IsSchemeEnabledOnTenant(AuthenticationScheme scheme)
        {
            switch (scheme.Name)
            {
                case "OpenIdConnect":
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsDeactivated, _session.GetTenantId());
                case "Microsoft":
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsDeactivated, _session.GetTenantId());
                case "Google":
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.Google_IsDeactivated, _session.GetTenantId());
                case "Twitter":
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.Twitter_IsDeactivated, _session.GetTenantId());
                case "Facebook":
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.Facebook_IsDeactivated, _session.GetTenantId());
                case "WsFederation":
                    return !_settingManager.GetSettingValueForTenant<bool>(AppSettings.ExternalLoginProvider.Tenant.WsFederation_IsDeactivated, _session.GetTenantId());
                default: return true;
            }
        }
    }
}
