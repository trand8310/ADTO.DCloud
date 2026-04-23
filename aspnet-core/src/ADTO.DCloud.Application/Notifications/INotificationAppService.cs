using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Notifications;
using ADTO.DCloud.Notifications.Dto;

namespace ADTO.DCloud.Notifications
{
    public interface INotificationAppService : IApplicationService
    {
        Task<GetNotificationsOutput> GetUserNotifications(GetUserNotificationsInput input);
        
        Task<SetNotificationAsReadOutput> SetAllAvailableVersionNotificationAsRead();
        
        Task SetAllNotificationsAsRead();

        Task<SetNotificationAsReadOutput> SetNotificationAsRead(EntityDto<Guid> input);

        Task<GetNotificationSettingsOutput> GetNotificationSettings();

        Task UpdateNotificationSettings(UpdateNotificationSettingsInput input);

        Task DeleteNotification(EntityDto<Guid> input);

        Task DeleteAllUserNotifications(DeleteAllUserNotificationsInput input);

        Task CreateMassNotification(CreateMassNotificationInput input);
        
        Task CreateNewVersionReleasedNotification();
        
        Task<bool> ShouldUserUpdateApp();

        List<string> GetAllNotifiers();

        Task<GetPublishedNotificationsOutput> GetNotificationsPublishedByUser(GetPublishedNotificationsInput input);
    }
}
