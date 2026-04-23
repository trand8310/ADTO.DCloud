using ADTOSharp.Authorization.Roles;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;
using System;

namespace ADTOSharp.Organizations
{
    /// <summary>
    /// Removes the role from all organization units when a role is deleted.
    /// </summary>
    public class OrganizationUnitRoleRemover : 
        IEventHandler<EntityDeletedEventData<ADTOSharpRoleBase>>, 
        ITransientDependency
    {
        private readonly IRepository<OrganizationUnitRole, Guid> _organizationUnitRoleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public OrganizationUnitRoleRemover(
            IRepository<OrganizationUnitRole, Guid> organizationUnitRoleRepository, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        
        public virtual void HandleEvent(EntityDeletedEventData<ADTOSharpRoleBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(eventData.Entity.TenantId))
                {
                    _organizationUnitRoleRepository.Delete(
                        uou => uou.RoleId == eventData.Entity.Id
                    );
                }
            });
        }
    }
}
