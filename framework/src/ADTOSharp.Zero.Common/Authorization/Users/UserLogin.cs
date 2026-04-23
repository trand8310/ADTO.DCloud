using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;

namespace ADTOSharp.Authorization.Users
{
    /// <summary>
    /// 三方登录
    /// </summary>
    [Table("ADTOSharpUserLogins"), Description("三方登录")]
    public class UserLogin : Entity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of <see cref="LoginProvider"/> property.
        /// </summary>
        public const int MaxLoginProviderLength = 128;

        /// <summary>
        /// Maximum length of <see cref="ProviderKey"/> property.
        /// </summary>
        public const int MaxProviderKeyLength = 256;

        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// Id of the User.
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Login Provider.
        /// </summary>
        [Required]
        [StringLength(MaxLoginProviderLength)]
        public virtual string LoginProvider { get; set; }

        /// <summary>
        /// Key in the <see cref="LoginProvider"/>.
        /// </summary>
        [Required]
        [StringLength(MaxProviderKeyLength)]
        public virtual string ProviderKey { get; set; }

        public UserLogin()
        {
            
        }

        public UserLogin(Guid? tenantId, Guid userId, string loginProvider, string providerKey)
        {
            TenantId = tenantId;
            UserId = userId;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }
    }
}
