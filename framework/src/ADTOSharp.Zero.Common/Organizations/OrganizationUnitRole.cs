using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTOSharp.Organizations
{
    /// <summary>
    /// 表示用户及组织架构的角色关系。
    /// </summary>
    [Table("ADTOSharpOrganizationUnitRoles"),Description("组织架构角色")]
    public class OrganizationUnitRole : CreationAuditedEntity<Guid>, IMayHaveTenant, ISoftDelete
    {
        /// <summary>
        /// TenantId of this entity.
        /// </summary>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// Id of the Role.
        /// </summary>
        public virtual Guid RoleId { get; set; }

        /// <summary>
        /// Id of the <see cref="OrganizationUnit"/>.
        /// </summary>
        public virtual Guid OrganizationUnitId { get; set; }

        /// <summary>
        /// Specifies if the organization is soft deleted or not.
        /// </summary>
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationUnitRole"/> class.
        /// </summary>
        public OrganizationUnitRole()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationUnitRole"/> class.
        /// </summary>
        /// <param name="tenantId">TenantId</param>
        /// <param name="roleId">Id of the User.</param>
        /// <param name="organizationUnitId">Id of the <see cref="OrganizationUnit"/>.</param>
        public OrganizationUnitRole(Guid? tenantId, Guid roleId, Guid organizationUnitId)
        {
            TenantId = tenantId;
            RoleId = roleId;
            OrganizationUnitId = organizationUnitId;
        }
    }
}
