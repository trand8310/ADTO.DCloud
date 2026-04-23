using ADTO.DCloud.Debugging;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Net.Mail;
using ADTOSharp.Extensions;
using System.Threading.Tasks;

namespace ADTO.DCloud.Net.Emailing
{
    public class EmailSettingsChecker : IEmailSettingsChecker, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        public EmailSettingsChecker(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public bool EmailSettingsValid()
        {
            if (DebugHelper.IsDebug)
            {
                return true;
            }
            
            var defaultFromAddress = _settingManager.GetSettingValue(EmailSettingNames.DefaultFromAddress);
            var host = _settingManager.GetSettingValue(EmailSettingNames.Smtp.Host);

            if (defaultFromAddress.IsNullOrEmpty() || host.IsNullOrEmpty())
            {
                return false;
            }

            // these are dummy values ASP.NET Zero creates initially
            if (defaultFromAddress == "admin@mydomain.com" || host == "mydomain.com mailer")
            {
                return false;
            }

            if (_settingManager.GetSettingValue<bool>(EmailSettingNames.Smtp.UseDefaultCredentials))
            {
                return true;
            }

            if (_settingManager.GetSettingValue(EmailSettingNames.Smtp.UserName).IsNullOrEmpty() ||
                _settingManager.GetSettingValue(EmailSettingNames.Smtp.Password).IsNullOrEmpty()
            )
            {
                return false;
            }

            return true;
        }

        public async Task<bool> EmailSettingsValidAsync()
        {
            if (DebugHelper.IsDebug)
            {
                return true;
            }
            
            var defaultFromAddress = await _settingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromAddress);
            var host = await _settingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Host);

            if (defaultFromAddress.IsNullOrEmpty() || host.IsNullOrEmpty())
            {
                return false;
            }

            // these are dummy values ASP.NET Zero creates initially
            if (defaultFromAddress == "admin@mydomain.com" || host == "mydomain.com mailer")
            {
                return false;
            }

            if (await _settingManager.GetSettingValueAsync<bool>(EmailSettingNames.Smtp.UseDefaultCredentials))
            {
                return true;
            }

            if (
                (await _settingManager.GetSettingValueAsync(EmailSettingNames.Smtp.UserName)).IsNullOrEmpty() ||
                (await _settingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Password)).IsNullOrEmpty()
            )
            {
                return false;
            }

            return true;
        }
    }
}