using MailKit.Security;

namespace ADTOSharp.MailKit
{
    public class ADTOSharpMailKitConfiguration : IADTOSharpMailKitConfiguration
    {
        public SecureSocketOptions? SecureSocketOption { get; set; }
    }
}
