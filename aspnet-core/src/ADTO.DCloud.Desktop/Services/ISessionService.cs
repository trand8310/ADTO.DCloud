using ADTO.DCloud.Desktop.Models;

namespace ADTO.DCloud.Desktop.Services;

public interface ISessionService
{
    UserSession? Current { get; }
    bool IsAuthenticated { get; }
    void SignIn(UserSession session);
    void SignOut();
}
