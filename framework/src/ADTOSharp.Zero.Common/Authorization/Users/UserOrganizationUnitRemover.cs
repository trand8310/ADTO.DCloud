using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;
using System;

namespace ADTOSharp.Authorization.Users
{
    /// <summary>
    /// Removes the user from all organization units when a user is deleted.
    /// </summary>
    public class UserOrganizationUnitRemover : 
        IEventHandler<EntityDeletedEventData<ADTOSharpUserBase>>, 
        ITransientDependency
    {
        private readonly IRepository<UserOrganizationUnit, Guid> _userOrganizationUnitRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserOrganizationUnitRemover(
            IRepository<UserOrganizationUnit, Guid> userOrganizationUnitRepository, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        
        public virtual void HandleEvent(EntityDeletedEventData<ADTOSharpUserBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(eventData.Entity.TenantId))
                {
                    _userOrganizationUnitRepository.Delete(
                        uou => uou.UserId == eventData.Entity.Id
                    );
                }
            });
        }
    }
}
