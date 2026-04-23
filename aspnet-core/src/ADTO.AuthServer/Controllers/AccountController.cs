using ADTOSharp.Authorization.Users;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Notifications;
using ADTOSharp.Web.Models;
using ADTO.DCloud.Controllers;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Identity;
using ADTO.DCloud.Sessions;
using Microsoft.AspNetCore.Mvc;
using ADTOSharp.Authorization;
using ADTO.AuthServer.Models.Account;


namespace ADTO.AuthServer.Controllers
{
    public class AccountController : DCloudControllerBase
    {
        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ADTOSharpLoginResultTypeHelper _loginResultTypeHelper;
        private readonly LogInManager _logInManager;
        private readonly SignInManager _signInManager;
        private readonly ISessionAppService _sessionAppService;
        private readonly ITenantCache _tenantCache;
        private readonly INotificationPublisher _notificationPublisher;

        public AccountController(
            UserManager userManager,
            IMultiTenancyConfig multiTenancyConfig,
            TenantManager tenantManager,
            IUnitOfWorkManager unitOfWorkManager,
            ADTOSharpLoginResultTypeHelper loginResultTypeHelper,
            LogInManager logInManager,
            SignInManager signInManager,
            ISessionAppService sessionAppService,
            ITenantCache tenantCache,
            INotificationPublisher notificationPublisher)
        {
            _userManager = userManager;
            _multiTenancyConfig = multiTenancyConfig;
            _tenantManager = tenantManager;
            _unitOfWorkManager = unitOfWorkManager;
            _loginResultTypeHelper = loginResultTypeHelper;
            _logInManager = logInManager;
            _signInManager = signInManager;
            _sessionAppService = sessionAppService;
            _tenantCache = tenantCache;
            _notificationPublisher = notificationPublisher;
        }

        #region Login / Logout

        public ActionResult Login(string userNameOrEmailAddress = "", string returnUrl = "", string successMessage = "")
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = GetAppHomeUrl();
            }

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
            });
        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<JsonResult> Login(LoginViewModel loginModel, string returnUrl = "", string returnUrlHash = "")
        {
            returnUrl = NormalizeReturnUrl(returnUrl);
            if (!string.IsNullOrWhiteSpace(returnUrlHash))
            {
                returnUrl = returnUrl + returnUrlHash;
            }

            var loginResult = await GetLoginResultAsync(loginModel.Username, loginModel.Password, GetTenancyNameOrNull());

            await _signInManager.SignInAsync(loginResult.Identity, loginModel.RememberMe);
            await UnitOfWorkManager.Current.SaveChangesAsync();

            return Json(new AjaxResponse {  });
        }
        
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        private async Task<ADTOSharpLoginResult<Tenant, User>> GetLoginResultAsync(string userName, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(userName, password, tenancyName);

            switch (loginResult.Result)
            {
                case ADTOSharpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _loginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, userName, tenancyName);
            }
        }

        #endregion

 
        #region 403 Forbidden
        
        [Route("/Account/Forbidden")]
        public ActionResult Error403()
        {
            return View();
        }
        
        #endregion
        
        #region Helpers

        public ActionResult RedirectToAppHome()
        {
            return RedirectToAction("Index", "Home");
        }

        public string GetAppHomeUrl()
        {
            return Url.Action("Index", "About");
        }

        #endregion
 

        #region Common

        private string GetTenancyNameOrNull()
        {
            if (!ADTOSharpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(ADTOSharpSession.TenantId.Value)?.TenancyName;
        }

        private string NormalizeReturnUrl(string returnUrl, Func<string> defaultValueBuilder = null)
        {
            if (defaultValueBuilder == null)
            {
                defaultValueBuilder = GetAppHomeUrl;
            }

            if (returnUrl.IsNullOrEmpty())
            {
                return defaultValueBuilder();
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }

            return defaultValueBuilder();
        }

        #endregion
 
    }
}

