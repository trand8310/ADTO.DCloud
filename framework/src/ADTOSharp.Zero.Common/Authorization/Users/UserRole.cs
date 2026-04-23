using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTOSharp.Authorization.Users
{
    /// <summary>
    /// 表示用户的角色记录。
    /// </summary>
    [Table("ADTOSharpUserRoles"), Description("用户角色")]
    public class UserRole : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// User id.
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Role id.
        /// </summary>
        public virtual Guid RoleId { get; set; }

        /// <summary>
        /// Creates a new <see cref="UserRole"/> object.
        /// </summary>
        public UserRole()
        {

        }

        /// <summary>
        /// Creates a new <see cref="UserRole"/> object.
        /// </summary>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="userId">User id</param>
        /// <param name="roleId">Role id</param>
        public UserRole(Guid? tenantId, Guid userId, Guid roleId)
        {
            TenantId = tenantId;
            UserId = userId;
            RoleId = roleId;
        }
    }
}