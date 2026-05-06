using ADTO.DCloud.Desktop.Models;

namespace ADTO.DCloud.Desktop.Services;

public interface IAuthApiClient
{
    Task<UserSession> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default);
}
