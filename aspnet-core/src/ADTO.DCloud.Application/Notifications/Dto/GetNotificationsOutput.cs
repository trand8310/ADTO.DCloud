using System.Collections.Generic;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Notifications;

namespace ADTO.DCloud.Notifications.Dto
{
    public class GetNotificationsOutput : PagedResultDto<UserNotification>
    {
        public int UnreadCount { get; set; }

        public GetNotificationsOutput(int totalCount, int unreadCount, List<UserNotification> notifications)
            :base(totalCount, notifications)
        {
            UnreadCount = unreadCount;
        }
    }
}