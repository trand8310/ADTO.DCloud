using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Caching;
using System;

namespace ADTOSharp.Authorization.Users
{
    public class ADTOSharpUserPermissionCacheItemInvalidator :
        IEventHandler<EntityChangedEventData<UserPermissionSetting>>,
        IEventHandler<EntityChangedEventData<UserRole>>,
        IEventHandler<EntityChangedEventData<UserOrganizationUnit>>,
        IEventHandler<EntityDeletedEventData<ADTOSharpUserBase>>,
        IEventHandler<EntityChangedEventData<OrganizationUnitRole>>,
        ITransientDependency
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<UserOrganizationUnit, Guid> _userOrganizationUnitRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ADTOSharpUserPermissionCacheItemInvalidator(
            ICacheManager cacheManager, 
            IRepository<UserOrganizationUnit, Guid> userOrganizationUnitRepository, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            _cacheManager = cacheManager;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void HandleEvent(EntityChangedEventData<UserPermissionSetting> eventData)
        {
            var cacheKey = eventData.Entity.UserId + "@" + (eventData.Entity.TenantId ?? Guid.Empty);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityChangedEventData<UserRole> eventData)
        {
            var cacheKey = eventData.Entity.UserId + "@" + (eventData.Entity.TenantId ?? Guid.Empty);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityChangedEventData<UserOrganizationUnit> eventData)
        {
            var cacheKey = eventData.Entity.UserId + "@" + (eventData.Entity.TenantId ?? Guid.Empty);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityDeletedEventData<ADTOSharpUserBase> eventData)
        {
            var cacheKey = eventData.Entity.Id + "@" + (eventData.Entity.TenantId ?? Guid.Empty);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public virtual void HandleEvent(EntityChangedEventData<OrganizationUnitRole> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(eventData.Entity.TenantId))
                {
                    var users = _userOrganizationUnitRepository.GetAllList(userOu =>
                        userOu.OrganizationUnitId == eventData.Entity.OrganizationUnitId
                    );

                    foreach (var userOrganizationUnit in users)
                    {
                        var cacheKey = userOrganizationUnit.UserId + "@" + (eventData.Entity.TenantId ?? Guid.Empty);
                        _cacheManager.GetUserPermissionCache().Remove(cacheKey);
                    }
                }
            });
        }
    }
}
