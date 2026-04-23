using System.Threading.Tasks;

namespace ADTO.DCloud.Authentication.External;

public interface IExternalAuthProviderApi
{
    ExternalLoginProviderInfo ProviderInfo { get; }
    Task<ExternalAuthUserInfo> GetUserInfo(string accessCode);
    void Initialize(ExternalLoginProviderInfo providerInfo);
    Task<bool> IsValidUser(string userId, string accessCode);
}
