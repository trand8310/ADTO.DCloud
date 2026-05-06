using ADTO.DCloud.Desktop.Models;

namespace ADTO.DCloud.Desktop.Services;

public sealed class SessionService : ISessionService
{
    public UserSession? Current { get; private set; }
    public bool IsAuthenticated => Current is not null;

    public void SignIn(UserSession session) => Current = session;

    public void SignOut() => Current = null;
}
