using System.Threading;
using System.Threading.Tasks;
using OpenIddict.Abstractions;

namespace ADTO.OpenIddict.Applications;

public interface IADTOApplicationManager : IOpenIddictApplicationManager
{
    ValueTask<string> GetFrontChannelLogoutUriAsync(object application, CancellationToken cancellationToken = default);

    ValueTask<string> GetClientUriAsync(object application, CancellationToken cancellationToken = default);

    ValueTask<string> GetLogoUriAsync(object application, CancellationToken cancellationToken = default);
}
