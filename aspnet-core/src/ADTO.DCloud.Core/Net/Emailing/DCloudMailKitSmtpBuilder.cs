
using ADTOSharp.MailKit;
using ADTOSharp.Localization;
using ADTOSharp.Net.Mail.Smtp;
using MailKit.Net.Smtp;
using ADTOSharp.UI;


namespace ADTO.DCloud.Net.Emailing
{
    public class DCloudMailKitSmtpBuilder : DefaultMailKitSmtpBuilder
    {
        private readonly ILocalizationManager _localizationManager;
        private readonly IEmailSettingsChecker _emailSettingsChecker;
        
        public DCloudMailKitSmtpBuilder(
            ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration,
            IADTOSharpMailKitConfiguration mailKitConfiguration,
            ILocalizationManager localizationManager, 
            IEmailSettingsChecker emailSettingsChecker) : base(smtpEmailSenderConfiguration, mailKitConfiguration)
        {
            _localizationManager = localizationManager;
            _emailSettingsChecker = emailSettingsChecker;
        }

        protected override void ConfigureClient(SmtpClient client)
        {
            if (!_emailSettingsChecker.EmailSettingsValid())
            {
                throw new UserFriendlyException(L("SMTPSettingsNotProvidedWarningText"));
            }
            
            client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            base.ConfigureClient(client);
        }

        private string L(string name)
        {
            return _localizationManager.GetString(DCloudConsts.LocalizationSourceName, name);
        }
    }
}
