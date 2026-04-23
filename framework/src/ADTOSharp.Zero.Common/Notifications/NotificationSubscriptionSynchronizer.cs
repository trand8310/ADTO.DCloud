using System;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;

namespace ADTOSharp.Notifications
{
    public class NotificationSubscriptionSynchronizer : IEventHandler<EntityDeletedEventData<ADTOSharpUserBase>>,
        ITransientDependency
    {
        private readonly IRepository<NotificationSubscriptionInfo, Guid> _notificationSubscriptionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public NotificationSubscriptionSynchronizer(
            IRepository<NotificationSubscriptionInfo, Guid> notificationSubscriptionRepository,
            IUnitOfWorkManager unitOfWorkManager
        )
        {
            _notificationSubscriptionRepository = notificationSubscriptionRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual void HandleEvent(EntityDeletedEventData<ADTOSharpUserBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(eventData.Entity.TenantId))
                {
                    _notificationSubscriptionRepository.Delete(x => x.UserId == eventData.Entity.Id);
                }
            });
        }
    }
}
