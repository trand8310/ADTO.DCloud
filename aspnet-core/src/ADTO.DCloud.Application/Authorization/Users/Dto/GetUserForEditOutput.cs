using System;
using System.Collections.Generic;
using ADTO.DCloud.Authorization.Posts.Dto;
using ADTO.DCloud.Organizations.Dto;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class GetUserForEditOutput
    {
        /// <summary>
        /// 用户图像
        /// </summary>
        public Guid? ProfilePictureId { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserEditDto User { get; set; }
        /// <summary>
        /// 用户所属角色
        /// </summary>
        public UserRoleDto[] Roles { get; set; }
        /// <summary>
        /// 系统所有的组织数据
        /// </summary>
        public List<OrganizationUnitDto> AllOrganizationUnits { get; set; }
        /// <summary>
        /// 用户所属组织代码
        /// </summary>
        public List<string> MemberedOrganizationUnits { get; set; }
        
        /// <summary>
        /// 用户名属允许的字符
        /// </summary>
        public string AllowedUserNameCharacters { get; set; }
        /// <summary>
        /// SMTP设置
        /// </summary>
        public bool IsSMTPSettingsProvided { get; set; }
        /// <summary>
        /// 用户所属岗位
        /// </summary>
        public List<PostDto> MemberedPosts { get; set; }
        /// <summary>
        /// 用户权限
        /// </summary>
        public List<string> GrantedPermissionNames { get; set; }
    }
}