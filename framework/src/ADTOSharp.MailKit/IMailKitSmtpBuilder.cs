using System;
using MailKit.Net.Smtp;

namespace ADTOSharp.MailKit
{
    public interface IMailKitSmtpBuilder
    {
        SmtpClient Build();
    }
}