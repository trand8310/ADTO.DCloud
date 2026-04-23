using MailKit.Security;

namespace ADTOSharp.MailKit
{
    public interface IADTOSharpMailKitConfiguration
    {
        SecureSocketOptions? SecureSocketOption { get; set; }
    }
}