using System;
using System.Collections.Generic;
using System.Linq;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Extensions;

namespace ADTOSharp.Notifications
{
    /// <summary>
    /// Represents a notification sent to a user.
    /// </summary>
    [Serializable]
    public class UserNotification : EntityDto<Guid>, IUserIdentifier
    {
        /// <summary>
        /// TenantId.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Current state of the user notification.
        /// </summary>
        public UserNotificationState State { get; set; }

        /// <summary>
        /// The notification.
        /// </summary>
        public TenantNotification Notification { get; set; }

        /// <summary>
        /// which realtime notifiers should handle this notification
        /// </summary>
        public string TargetNotifiers { get; set; }

        public List<string> TargetNotifiersList => TargetNotifiers.IsNullOrWhiteSpace()
            ? new List<string>()
            : TargetNotifiers.Split(NotificationInfo.NotificationTargetSeparator).ToList();

    }
}