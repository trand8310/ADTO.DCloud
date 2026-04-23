using ADTO.AspNetCore.OpenIddict.Claims;
using ADTO.AspNetCore.OpenIddict.Controllers;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.MultiTenancy;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using OpenIddict.Abstractions;

namespace ADTO.DCloud.Web.OpenIddict.Controllers
{
    public class UserInfoController : UserInfoController<Tenant, Role, User>
    {
        public UserInfoController(ADTOSharpSignInManager<Tenant, Role, User> signInManager,
            ADTOSharpUserManager<Role, User> userManager, IOpenIddictApplicationManager applicationManager,
            IOpenIddictAuthorizationManager authorizationManager, IOpenIddictScopeManager scopeManager,
            IOpenIddictTokenManager tokenManager,
            ADTOOpenIddictClaimsPrincipalManager openIddictClaimsPrincipalManager) : base(signInManager, userManager,
            applicationManager, authorizationManager, scopeManager, tokenManager, openIddictClaimsPrincipalManager)
        {
        }
    }
}