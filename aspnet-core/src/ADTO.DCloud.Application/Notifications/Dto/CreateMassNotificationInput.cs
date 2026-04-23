using ADTOSharp.Notifications;
using System;

namespace ADTO.DCloud.Notifications.Dto
{
    public class CreateMassNotificationInput
    {
        public string Message { get; set; }

        public NotificationSeverity Severity { get; set; }

        public Guid[] UserIds { get; set; }

        public Guid[] OrganizationUnitIds { get; set; }
        
        public string[] TargetNotifiers { get; set; }
    }
}