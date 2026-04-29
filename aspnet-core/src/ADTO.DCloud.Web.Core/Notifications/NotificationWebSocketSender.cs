using System.Threading.Tasks;
using ADTO.DCloud.Notifications.WS;
using ADTOSharp.Dependency;
using ADTOSharp.Notifications;

namespace ADTO.DCloud.Notifications;

public class NotificationWebSocketSender : INotificationWebSocketSender, ITransientDependency
{
    private readonly INotificationWebSocketConnectionManager _connectionManager;

    public NotificationWebSocketSender(INotificationWebSocketConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public Task SendAsync(UserNotification userNotification)
    {
        return _connectionManager.SendAsync(userNotification.ToUserIdentifier(), "getNotification", userNotification);
    }
}
