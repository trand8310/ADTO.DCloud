using System;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Application.Editions;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTOSharp.MultiTenancy
{
    /// <summary>
    /// Represents a Tenant of the application.
    /// </summary>
    public abstract class ADTOSharpTenant<TUser> : ADTOSharpTenantBase, IFullAudited<TUser>
        where TUser : ADTOSharpUserBase
    {
        /// <summary>
        /// Current <see cref="Edition"/> of the Tenant.
        /// </summary>
        public virtual Edition Edition { get; set; }
        public virtual Guid? EditionId { get; set; }

        /// <summary>
        /// Reference to the creator user of this entity.
        /// </summary>
        public virtual TUser CreatorUser { get; set; }

        /// <summary>
        /// Reference to the last modifier user of this entity.
        /// </summary>
        public virtual TUser LastModifierUser { get; set; }

        /// <summary>
        /// Reference to the deleter user of this entity.
        /// </summary>
        public virtual TUser DeleterUser { get; set; }

        /// <summary>
        /// Creates a new tenant.
        /// </summary>
        protected ADTOSharpTenant()
        {
            IsActive = true;
        }

        /// <summary>
        /// Creates a new tenant.
        /// </summary>
        /// <param name="tenancyName">UNIQUE name of this Tenant</param>
        /// <param name="name">Display name of the Tenant</param>
        protected ADTOSharpTenant(string tenancyName, string name)
            : this()
        {
            TenancyName = tenancyName;
            Name = name;
        }
    }
}
