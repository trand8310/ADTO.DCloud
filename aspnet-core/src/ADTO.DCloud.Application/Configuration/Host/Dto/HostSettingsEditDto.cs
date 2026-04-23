using System.ComponentModel.DataAnnotations;
using ADTO.DCloud.Configuration.Dto;
using ADTO.DCloud.Configuration.Tenants.Dto;

namespace ADTO.DCloud.Configuration.Host.Dto
{
    public class HostSettingsEditDto
    {
        /// <summary>
        /// 常规选项
        /// </summary>
        [Required]
        public GeneralSettingsEditDto General { get; set; }
        /// <summary>
        /// 用户管理相关选项
        /// </summary>
        [Required]
        public HostUserManagementSettingsEditDto UserManagement { get; set; }
        /// <summary>
        /// 邮箱设置选项
        /// </summary>
        [Required]
        public EmailSettingsEditDto Email { get; set; }
        /// <summary>
        /// 租户管理选项
        /// </summary>
        [Required]
        public TenantManagementSettingsEditDto TenantManagement { get; set; }
        /// <summary>
        /// 安全设置选项
        /// </summary>
        [Required]
        public SecuritySettingsEditDto Security { get; set; }

        //public HostBillingSettingsEditDto Billing { get; set; }
        /// <summary>
        /// 其它设置选项
        /// </summary>
        public OtherSettingsEditDto OtherSettings { get; set; }


        public HostSettingsEditDto()
        {

        }
    }
}