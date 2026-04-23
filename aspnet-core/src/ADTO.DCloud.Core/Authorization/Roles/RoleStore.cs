using ADTOSharp.Authorization.Roles;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTO.DCloud.Authorization.Users;
using System;

namespace ADTO.DCloud.Authorization.Roles
{
    public class RoleStore : ADTOSharpRoleStore<Role, User>
    {
        public RoleStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Role, Guid> roleRepository,
            IRepository<RolePermissionSetting, Guid> rolePermissionSettingRepository)
            : base(
                unitOfWorkManager,
                roleRepository,
                rolePermissionSettingRepository)
        {
        }
    }
}
