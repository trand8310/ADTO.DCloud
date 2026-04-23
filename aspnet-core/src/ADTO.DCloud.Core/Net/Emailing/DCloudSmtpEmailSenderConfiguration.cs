

using ADTOSharp.Configuration;
using ADTOSharp.Net.Mail;
using ADTOSharp.Net.Mail.Smtp;
using ADTOSharp.Runtime.Security;
using System.Xml.Linq;

namespace ADTO.DCloud.Net.Emailing
{
    public class DCloudSmtpEmailSenderConfiguration : SmtpEmailSenderConfiguration
    {
        public DCloudSmtpEmailSenderConfiguration(ISettingManager settingManager) : base(settingManager)
        {
            //Adto#21$25
            //string settingValue = SettingManager.GetSettingValue(EmailSettingNames.Smtp.Password);

        }


        public override string Password => SimpleStringCipher.Instance.Decrypt(GetNotEmptySettingValue(EmailSettingNames.Smtp.Password));


    }
}
