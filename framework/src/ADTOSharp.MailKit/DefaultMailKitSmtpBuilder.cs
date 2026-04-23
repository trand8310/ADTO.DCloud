using ADTOSharp.Dependency;
using ADTOSharp.Net.Mail.Smtp;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace ADTOSharp.MailKit
{
    public class DefaultMailKitSmtpBuilder : IMailKitSmtpBuilder, ITransientDependency
    {
        private readonly ISmtpEmailSenderConfiguration _smtpEmailSenderConfiguration;
        private readonly IADTOSharpMailKitConfiguration _mailKitConfiguration;

        public DefaultMailKitSmtpBuilder(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration, IADTOSharpMailKitConfiguration mailKitConfiguration)
        {
            _smtpEmailSenderConfiguration = smtpEmailSenderConfiguration;
            _mailKitConfiguration = mailKitConfiguration;
        }

        public virtual SmtpClient Build()
        {
            var client = new SmtpClient();

            try
            {
                ConfigureClient(client);
                return client;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        protected virtual void ConfigureClient(SmtpClient client)
        {
            client.Connect(
                _smtpEmailSenderConfiguration.Host,
                _smtpEmailSenderConfiguration.Port,
                GetSecureSocketOption()
            );

            if (_smtpEmailSenderConfiguration.UseDefaultCredentials)
            {
                return;
            }

            client.Authenticate(
                _smtpEmailSenderConfiguration.UserName,
                _smtpEmailSenderConfiguration.Password
            );
        }

        protected virtual SecureSocketOptions GetSecureSocketOption()
        {
            if (_mailKitConfiguration.SecureSocketOption.HasValue)
            {
                return _mailKitConfiguration.SecureSocketOption.Value;
            }

            return _smtpEmailSenderConfiguration.EnableSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;
        }
    }
}