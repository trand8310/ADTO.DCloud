using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTOSharp.Authorization.Roles
{
    /// <summary>
    /// 角色声明
    /// </summary>
    [Table("ADTOSharpRoleClaims"), Description("角色声明")]
    public class RoleClaim : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of the <see cref="ClaimType"/> property.
        /// </summary>
        public const int MaxClaimTypeLength = 256;
        /// <summary>
        /// 租户ID
        /// </summary>
        public virtual Guid? TenantId { get; set; }
        /// <summary>
        /// 角色Id
        /// </summary>
        public virtual Guid RoleId { get; set; }
        /// <summary>
        /// 声明类型
        /// </summary>
        [StringLength(MaxClaimTypeLength)]
        public virtual string ClaimType { get; set; }
        /// <summary>
        /// 声明的值
        /// </summary>
        public virtual string ClaimValue { get; set; }

        public RoleClaim()
        {
            
        }

        public RoleClaim(ADTOSharpRoleBase role, Claim claim)
        {
            TenantId = role.TenantId;
            RoleId = role.Id;
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }
}
