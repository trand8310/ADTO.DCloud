using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTOSharp.Authorization.Users
{
    /// <summary>
    /// 用户声明
    /// </summary>
    [Table("ADTOSharpUserClaims"), Description("用户声明")]
    public class UserClaim : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of the <see cref="ClaimType"/> property.
        /// </summary>
        public const int MaxClaimTypeLength = 256;

        public virtual Guid? TenantId { get; set; }

        public virtual Guid UserId { get; set; }

        [StringLength(MaxClaimTypeLength)]
        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }

        public UserClaim()
        {
            
        }

        public UserClaim(ADTOSharpUserBase user, Claim claim)
        {
            TenantId = user.TenantId;
            UserId = user.Id;
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }
}
