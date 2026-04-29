using System.Threading.Tasks;
using ADTOSharp.Dependency;

namespace ADTOSharp.Notifications;

public class NullNotificationWebSocketSender : INotificationWebSocketSender, ISingletonDependency
{
    public Task SendAsync(UserNotification userNotification)
    {
        return Task.CompletedTask;
    }
}
