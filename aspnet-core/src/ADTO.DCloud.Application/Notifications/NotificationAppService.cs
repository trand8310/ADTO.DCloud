using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.BackgroundJobs;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Configuration;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Notifications;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Notifications.Dto;
using ADTO.DCloud.Organizations;

namespace ADTO.DCloud.Notifications;

/// <summary>
/// 通知服务
/// </summary>
[ADTOSharpAuthorize]
public class NotificationAppService : DCloudAppServiceBase, INotificationAppService
{
    #region Fields
    private readonly INotificationDefinitionManager _notificationDefinitionManager;
    private readonly IUserNotificationManager _userNotificationManager;
    private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
    private readonly IRepository<User, Guid> _userRepository;
    private readonly IRepository<OrganizationUnit, Guid> _organizationUnitRepository;
    private readonly IAppNotifier _appNotifier;
    private readonly IUserOrganizationUnitRepository _userOrganizationUnitRepository;
    private readonly INotificationConfiguration _notificationConfiguration;
    private readonly INotificationStore _notificationStore;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IRepository<UserNotificationInfo, Guid> _userNotificationRepository;
    #endregion

    #region Ctor

    public NotificationAppService(
        INotificationDefinitionManager notificationDefinitionManager,
        IUserNotificationManager userNotificationManager,
        INotificationSubscriptionManager notificationSubscriptionManager,
        IRepository<User, Guid> userRepository,
        IRepository<OrganizationUnit, Guid> organizationUnitRepository,
        IAppNotifier appNotifier,
        IUserOrganizationUnitRepository userOrganizationUnitRepository,
        INotificationConfiguration notificationConfiguration,
        INotificationStore notificationStore,
        IBackgroundJobManager backgroundJobManager,
        IRepository<UserNotificationInfo, Guid> userNotificationRepository)
    {
        _notificationDefinitionManager = notificationDefinitionManager;
        _userNotificationManager = userNotificationManager;
        _notificationSubscriptionManager = notificationSubscriptionManager;
        _userRepository = userRepository;
        _organizationUnitRepository = organizationUnitRepository;
        _appNotifier = appNotifier;
        _userOrganizationUnitRepository = userOrganizationUnitRepository;
        _notificationConfiguration = notificationConfiguration;
        _notificationStore = notificationStore;
        _backgroundJobManager = backgroundJobManager;
        _userNotificationRepository = userNotificationRepository;
    }
    #endregion

    #region Methods
    /// <summary>
    /// 获取当前用户的所有消息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisableAuditing]
    public async Task<GetNotificationsOutput> GetUserNotifications(GetUserNotificationsInput input)
    {
 
        var totalCount = await _userNotificationManager.GetUserNotificationCountAsync(
            ADTOSharpSession.ToUserIdentifier(), input.State, input.StartDate, input.EndDate
        );

        var unreadCount = await _userNotificationManager.GetUserNotificationCountAsync(
            ADTOSharpSession.ToUserIdentifier(), UserNotificationState.Unread, input.StartDate, input.EndDate
        );
        var notifications = await _userNotificationManager.GetUserNotificationsAsync(
            ADTOSharpSession.ToUserIdentifier(), input.State, input.PageNumber, input.PageSize, input.StartDate,
            input.EndDate
        );

        return new GetNotificationsOutput(totalCount, unreadCount, notifications);
    }
    
    /// <summary>
    /// 提醒用户存在新版本的通知,一现用于APP,小程序或其它第三方应用
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ShouldUserUpdateApp()
    {
        var notifications = await _userNotificationManager.GetUserNotificationsAsync(
            ADTOSharpSession.ToUserIdentifier(), UserNotificationState.Unread
        );
        
        return notifications.Any(x => x.Notification.NotificationName == AppNotificationNames.NewVersionAvailable);
    }
    
    /// <summary>
    /// 标记用户所有的关于版本的消息设置为已读,
    /// </summary>
    /// <returns></returns>
    public async Task<SetNotificationAsReadOutput> SetAllAvailableVersionNotificationAsRead()
    {
        var notifications = await _userNotificationManager.GetUserNotificationsAsync(
            ADTOSharpSession.ToUserIdentifier(), UserNotificationState.Unread
        );
        
        var filteredNotifications =  notifications
            .Where(x => x.Notification.NotificationName == AppNotificationNames.NewVersionAvailable)
            .ToList();
        
        if (!filteredNotifications.Any())
        {
            return new SetNotificationAsReadOutput(false);
        }

        foreach (var notification in filteredNotifications)
        {
            if (notification.State == UserNotificationState.Read)
            {
                continue;
            }

            await _userNotificationManager.UpdateUserNotificationStateAsync(
                notification.TenantId,
                notification.Id,
                UserNotificationState.Read
            );
        }
        
        return new SetNotificationAsReadOutput(true);
    }

    /// <summary>
    /// 设置用户所有的消息为已读
    /// </summary>
    /// <returns></returns>
    public async Task SetAllNotificationsAsRead()
    {
        await _userNotificationManager.UpdateAllUserNotificationStatesAsync(
            ADTOSharpSession.ToUserIdentifier(),
            UserNotificationState.Read
        );
    }
    /// <summary>
    /// 标记指定的消息为已读
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>

    public async Task<SetNotificationAsReadOutput> SetNotificationAsRead(EntityDto<Guid> input)
    {
        var userNotification =
            await _userNotificationManager.GetUserNotificationAsync(ADTOSharpSession.TenantId, input.Id);
        if (userNotification == null)
        {
            return new SetNotificationAsReadOutput(false);
        }

        if (userNotification.UserId != ADTOSharpSession.GetUserId())
        {
            throw new Exception(
                $"Given user notification id ({input.Id}) is not belong to the current user ({ADTOSharpSession.GetUserId()})"
            );
        }

        if (userNotification.State == UserNotificationState.Read)
        {
            return new SetNotificationAsReadOutput(false);
        }

        await _userNotificationManager.UpdateUserNotificationStateAsync(ADTOSharpSession.TenantId, input.Id,
            UserNotificationState.Read);
        return new SetNotificationAsReadOutput(true);
    }

    /// <summary>
    /// 获取与消息相关的设置
    /// </summary>
    /// <returns></returns>
    public async Task<GetNotificationSettingsOutput> GetNotificationSettings()
    {
        var output = new GetNotificationSettingsOutput();

        output.ReceiveNotifications =
            await SettingManager.GetSettingValueAsync<bool>(NotificationSettingNames.ReceiveNotifications);

        //Get general notifications, not entity related notifications.
        var notificationDefinitions =
            (await _notificationDefinitionManager.GetAllAvailableAsync(ADTOSharpSession.ToUserIdentifier())).Where(nd =>
                nd.EntityType == null);

        output.Notifications =
            ObjectMapper.Map<List<NotificationSubscriptionWithDisplayNameDto>>(notificationDefinitions);

        var subscribedNotifications = (await _notificationSubscriptionManager
                .GetSubscribedNotificationsAsync(ADTOSharpSession.ToUserIdentifier()))
            .Select(ns => ns.NotificationName)
            .ToList();

        output.Notifications.ForEach(n => n.IsSubscribed = subscribedNotifications.Contains(n.Name));

        return output;
    }
    /// <summary>
    /// 更新消息配置项
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateNotificationSettings(UpdateNotificationSettingsInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(ADTOSharpSession.ToUserIdentifier(),
            NotificationSettingNames.ReceiveNotifications, input.ReceiveNotifications.ToString());

        foreach (var notification in input.Notifications)
        {
            if (notification.IsSubscribed)
            {
                await _notificationSubscriptionManager.SubscribeAsync(ADTOSharpSession.ToUserIdentifier(),
                    notification.Name);
            }
            else
            {
                await _notificationSubscriptionManager.UnsubscribeAsync(ADTOSharpSession.ToUserIdentifier(),
                    notification.Name);
            }
        }
    }
    /// <summary>
    /// 删除某条消息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task DeleteNotification(EntityDto<Guid> input)
    {
        var notification = await _userNotificationManager.GetUserNotificationAsync(ADTOSharpSession.TenantId, input.Id);
        if (notification == null)
        {
            return;
        }

        if (notification.UserId != ADTOSharpSession.GetUserId())
        {
            throw new UserFriendlyException(L("ThisNotificationDoesntBelongToYou"));
        }

        await _userNotificationManager.DeleteUserNotificationAsync(ADTOSharpSession.TenantId, input.Id);
    }

    /// <summary>
    /// 删除当前用户给定条件下的所有消息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    public async Task DeleteAllUserNotifications(DeleteAllUserNotificationsInput input)
    {
        await _userNotificationManager.DeleteAllUserNotificationsAsync(
            ADTOSharpSession.ToUserIdentifier(),
            input.State,
            input.StartDate,
            input.EndDate);
    }
    /// <summary>
    /// 用户列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    //[ADTOSharpAuthorize(PermissionNames.Pages_Administration_MassNotification)]
    public async Task<PagedResultDto<MassNotificationUserLookupTableDto>> GetAllUserForLookupTable(
        GetAllForLookupTableInput input)
    {
        var query = _userRepository.GetAll()
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                e =>
                    (e.Name != null && e.Name.Contains(input.Filter)) ||
                    (e.EmailAddress != null && e.EmailAddress.Contains(input.Filter))
            );

        var totalCount = await query.CountAsync();

        var userList = await query
            .PageBy(input)
            .ToListAsync();

        var lookupTableDtoList = new List<MassNotificationUserLookupTableDto>();
        foreach (var user in userList)
        {
            lookupTableDtoList.Add(new MassNotificationUserLookupTableDto
            {
                Id = user.Id,
                DisplayName = user.Name + " (" + user.EmailAddress + ")"
            });
        }

        return new PagedResultDto<MassNotificationUserLookupTableDto>(
            totalCount,
            lookupTableDtoList
        );
    }
    /// <summary>
    /// 组织架构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    //[ADTOSharpAuthorize(PermissionNames.Pages_Administration_MassNotification)]
    public async Task<PagedResultDto<MassNotificationOrganizationUnitLookupTableDto>> GetAllOrganizationUnitForLookupTable(GetAllForLookupTableInput input)
    {
        var query = _organizationUnitRepository.GetAll()
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                e => e.DisplayName != null && e.DisplayName.Contains(input.Filter));

        var totalCount = await query.CountAsync();

        var organizationUnitList = await query
            .PageBy(input)
            .ToListAsync();

        var lookupTableDtoList = new List<MassNotificationOrganizationUnitLookupTableDto>();
        foreach (var organizationUnit in organizationUnitList)
        {
            lookupTableDtoList.Add(new MassNotificationOrganizationUnitLookupTableDto
            {
                Id = organizationUnit.Id,
                DisplayName = organizationUnit.DisplayName
            });
        }

        return new PagedResultDto<MassNotificationOrganizationUnitLookupTableDto>(
            totalCount,
            lookupTableDtoList
        );
    }
    /// <summary>
    /// 给一组用户发送一个消息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_MassNotification_Create)]
    public async Task CreateMassNotification(CreateMassNotificationInput input)
    {
        if (input.TargetNotifiers.IsNullOrEmpty())
        {
            throw new UserFriendlyException(L("MassNotificationTargetNotifiersFieldIsRequiredMessage"));
        }

        var userIds = new List<UserIdentifier>();

        if (!input.UserIds.IsNullOrEmpty())
        {
            userIds.AddRange(input.UserIds.Select(i => new UserIdentifier(ADTOSharpSession.TenantId, i)));
        }

        if (!input.OrganizationUnitIds.IsNullOrEmpty())
        {
            userIds.AddRange(
                await _userOrganizationUnitRepository.GetAllUsersInOrganizationUnitHierarchical(
                    input.OrganizationUnitIds)
            );
        }

        if (userIds.Count == 0)
        {
            if (input.OrganizationUnitIds.IsNullOrEmpty())
            {
                // tried to get users from organization, but could not find any user
                throw new UserFriendlyException(L("MassNotificationNoUsersFoundInOrganizationUnitMessage"));
            }

            throw new UserFriendlyException(L("MassNotificationUserOrOrganizationUnitFieldIsRequiredMessage"));
        }

        var targetNotifiers = new List<Type>();

        foreach (var notifier in _notificationConfiguration.Notifiers)
        {
            if (input.TargetNotifiers.Contains(notifier.FullName))
            {
                targetNotifiers.Add(notifier);
            }
        }

        await _appNotifier.SendMassNotificationAsync(
            input.Message,
            userIds.DistinctBy(u => u.UserId).ToArray(),
            input.Severity,
            targetNotifiers.ToArray()
        );
    }
    /// <summary>
    /// 创建一个新版本可用的消息提醒,并推送
    /// </summary>
    /// <returns></returns>
  //  [ADTOSharpAuthorize(PermissionNames.Pages_Administration_NewVersion_Create)]
    public async Task CreateNewVersionReleasedNotification()
    {
        var args = new SendNotificationToAllUsersArgs
        {
            NotificationName = AppNotificationNames.NewVersionAvailable,
            Message = L("NewVersionAvailableNotificationMessage")
        };

        await _backgroundJobManager.EnqueueAsync<SendNotificationToAllUsersBackgroundJob, SendNotificationToAllUsersArgs>(args);
    }

    /// <summary>
    /// 获取所有的消息发送提供者(邮件,短信,站内信...)
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllNotifiers()
    {
        return _notificationConfiguration.Notifiers.Select(n => n.FullName).ToList();
    }
    /// <summary>
    /// 获取当前用户的所有消息通知
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    //[ADTOSharpAuthorize(PermissionNames.Pages_Administration_MassNotification)]
    [ADTOSharpAuthorize]
    public async Task<GetPublishedNotificationsOutput> GetNotificationsPublishedByUser(
        GetPublishedNotificationsInput input)
    {

         
        return new GetPublishedNotificationsOutput(
            await _notificationStore.GetNotificationsPublishedByUserAsync(ADTOSharpSession.ToUserIdentifier(),
                AppNotificationNames.MassNotification, input.StartDate, input.EndDate)
        );
    }
    /// <summary>
    /// 获取当前租户下面所有的消息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize]
    public async Task<GetPublishedNotificationsOutput> GetNotificationsPublishedByTenant(
       GetPublishedNotificationsInput input)
    {
        return new GetPublishedNotificationsOutput(
            await _notificationStore.GetNotificationsPublishedByTenantAsync(ADTOSharpSession.ToUserIdentifier().TenantId,
                AppNotificationNames.MassNotification, input.StartDate, input.EndDate)
        );
    }


    #endregion
}
