using System.Threading.Tasks;

namespace ADTOSharp.Notifications;

public interface INotificationWebSocketSender
{
    Task SendAsync(UserNotification userNotification);
}
