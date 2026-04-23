using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;
using System;

namespace ADTOSharp.Authorization.Users
{
    /// <summary>
    /// Removes the user from all user roles when a user is deleted.
    /// </summary>
    public class UserRoleRemover :
        IEventHandler<EntityDeletedEventData<ADTOSharpUserBase>>,
        ITransientDependency
    {
        private readonly IRepository<UserRole, Guid> _userRoleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserRoleRemover(
            IUnitOfWorkManager unitOfWorkManager, 
            IRepository<UserRole, Guid> userRoleRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _userRoleRepository = userRoleRepository;
        }
        
        public virtual void HandleEvent(EntityDeletedEventData<ADTOSharpUserBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(eventData.Entity.TenantId))
                {
                    _userRoleRepository.Delete(
                        ur => ur.UserId == eventData.Entity.Id
                    );
                }
            });
        }
    }
}
