using Microsoft.AspNetCore.Mvc;
using ADTO.DCloud.Controllers;
using ADTO.DCloud.Web.Session;
using ADTOSharp.Configuration.Startup;
using ADTO.DCloud.Authorization.Accounts;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Identity;
using ADTOSharp.Auditing;
using ADTO.DCloud.Authorization.Accounts.Dto;
using ADTOSharp.UI;
using ADTOSharp.Authorization.Users;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Authorization;
using ADTO.AuthServer.Models.Ui;

namespace ADTO.AuthServer.Controllers
{
    public class UiController : DCloudControllerBase
    {
        private readonly IPerRequestSessionCache _sessionCache;
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly IAccountAppService _accountAppService;
        private readonly LogInManager _logInManager;
        private readonly SignInManager _signInManager;
        private readonly ADTOSharpLoginResultTypeHelper _loginResultTypeHelper;

        public UiController(
            IPerRequestSessionCache sessionCache,
            IMultiTenancyConfig multiTenancyConfig,
            IAccountAppService accountAppService,
            LogInManager logInManager,
            SignInManager signInManager,
            ADTOSharpLoginResultTypeHelper loginResultTypeHelper)
        {
            _sessionCache = sessionCache;
            _multiTenancyConfig = multiTenancyConfig;
            _accountAppService = accountAppService;
            _logInManager = logInManager;
            _signInManager = signInManager;
            _loginResultTypeHelper = loginResultTypeHelper;
        }

        [DisableAuditing]
        public async Task<IActionResult> Index()
        {
            var model = new HomePageModel
            {
                LoginInformation = await _sessionCache.GetCurrentSessionAsync(),
                IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled
            };

            if (model.LoginInformation?.User == null)
            {
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = "")
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnUrl = returnUrl;
            }

            await _signInManager.SignOutAsync();
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = "")
        {
            if (model.TenancyName != null)
            {
                var isTenantAvailable = await _accountAppService.IsTenantAvailable(new IsTenantAvailableInput
                {
                    TenancyName = model.TenancyName
                });

                switch (isTenantAvailable.State)
                {
                    case TenantAvailabilityState.InActive:
                        throw new UserFriendlyException(L("TenantIsNotActive", model.TenancyName));
                    case TenantAvailabilityState.NotFound:
                        throw new UserFriendlyException(L("ThereIsNoTenantDefinedWithName{0}", model.TenancyName));
                }
            }

            var loginResult = await GetLoginResultAsync(model.UserName, model.Password, model.TenancyName);

            if (loginResult.User.ShouldChangePasswordOnNextLogin)
            {
                throw new UserFriendlyException(L("RequiresPasswordChange"));
            }

            var signInResult = await _signInManager.SignInOrTwoFactorAsync(loginResult, model.RememberMe);

            if (signInResult.RequiresTwoFactor)
            {
                throw new UserFriendlyException(L("RequiresTwoFactorAuth"));
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index");
        }

        private async Task<ADTOSharpLoginResult<Tenant, User>> GetLoginResultAsync(string username, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(username, password, tenancyName);

            switch (loginResult.Result)
            {
                case ADTOSharpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _loginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, username, tenancyName);
            }
        }
    }
}

