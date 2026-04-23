using ADTOSharp;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace ADTO.DCloud.Authorization.Users
{
    [Table("RecentPasswords")]
    public class RecentPassword : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public virtual Guid? TenantId { get; set; }

        [Required]
        public virtual Guid UserId { get; set; }

        [Required]
        public virtual string Password { get; set; }

        public RecentPassword()
        {
            Id = SequentialGuidGenerator.Instance.Create();
        }
    }
}