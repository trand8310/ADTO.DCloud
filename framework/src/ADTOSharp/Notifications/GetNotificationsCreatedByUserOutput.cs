using ADTOSharp.Application.Services.Dto;
using System;

namespace ADTOSharp.Notifications
{
    public class GetNotificationsCreatedByUserOutput:EntityDto<Guid>
    {
        public string NotificationName { get; set; }

        public string Data { get; set; }

        public string DataTypeName { get; set; }

        public NotificationSeverity Severity { get; set; }

        public bool IsPublished { get; set; }
        
        public DateTime CreationTime { get; set; }
    }
}