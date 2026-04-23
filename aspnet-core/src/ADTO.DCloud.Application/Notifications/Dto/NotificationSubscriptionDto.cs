using System.ComponentModel.DataAnnotations;
using ADTOSharp.Notifications;

namespace ADTO.DCloud.Notifications.Dto
{
    public class NotificationSubscriptionDto
    {
        [Required]
        [MaxLength(NotificationInfo.MaxNotificationNameLength)]
        public string Name { get; set; }

        public bool IsSubscribed { get; set; }
    }
}