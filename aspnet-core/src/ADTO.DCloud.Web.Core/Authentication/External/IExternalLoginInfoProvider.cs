namespace ADTO.DCloud.Authentication.External;

public interface IExternalLoginInfoProvider
{
    string Name { get; }

    ExternalLoginProviderInfo GetExternalLoginInfo();
}
