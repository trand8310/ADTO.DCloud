using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Runtime.Security;

namespace ADTOSharp.MultiTenancy
{
    /// <summary>
    /// Base class for tenants.
    /// </summary>
    [Table("ADTOSharpTenants")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public abstract class ADTOSharpTenantBase : FullAuditedEntity<Guid>, IPassivable
    {
        /// <summary>
        /// Max length of the <see cref="TenancyName"/> property.
        /// </summary>
        public const int MaxTenancyNameLength = 64;

        /// <summary>
        /// Max length of the <see cref="ConnectionString"/> property.
        /// </summary>
        public const int MaxConnectionStringLength = 1024;

        /// <summary>
        /// "Default".
        /// </summary>
        public const string DefaultTenantName = "Default";

        /// <summary>
        /// "^[a-zA-Z][a-zA-Z0-9_-]{1,}$".
        /// </summary>
        public const string TenancyNameRegex = "^[a-zA-Z][a-zA-Z0-9_-]{1,}$";

        /// <summary>
        /// Max length of the <see cref="Name"/> property.
        /// </summary>
        public const int MaxNameLength = 128;

        /// <summary>
        /// Tenancy name. This property is the UNIQUE name of this Tenant.
        /// It can be used as subdomain name in a web application.
        /// </summary>
        [Required]
        [StringLength(MaxTenancyNameLength)]
        public virtual string TenancyName { get; set; }

        /// <summary>
        /// Display name of the Tenant.
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        /// ENCRYPTED connection string of the tenant database.
        /// Can be null if this tenant is stored in host database.
        /// Use <see cref="SimpleStringCipher"/> to encrypt/decrypt this.
        /// </summary>
        [StringLength(MaxConnectionStringLength)]
        public virtual string ConnectionString { get; set; }

        /// <summary>
        /// Is this tenant active?
        /// If as tenant is not active, no user of this tenant can use the application.
        /// </summary>
        public virtual bool IsActive { get; set; }
    }
}