using System;
using ADTOSharp.Notifications;
using ADTO.DCloud.Dto;

namespace ADTO.DCloud.Notifications.Dto
{
    public class GetUserNotificationsInput : PagedInputDto
    {
        public UserNotificationState? State { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}