using ADTO.DCloud.Authorization.Posts.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    //[AutoMapTo(typeof(User))]
    public class CreateOrUpdateUserInput
    {
        [Required]
        public UserEditDto User { get; set; }

        /// <summary>
        /// 分配的角色名称
        /// </summary>
        [Required]
        public string[] AssignedRoleNames { get; set; }

        /// <summary>
        /// 是否发送激活邮件
        /// </summary>
        public bool SendActivationEmail { get; set; }

        /// <summary>
        /// 是否自动设置随机密码
        /// </summary>
        public bool SetRandomPassword { get; set; }

        /// <summary>
        /// 所属组织
        /// </summary>
        public List<Guid> OrganizationUnits { get; set; }
        /// <summary>
        /// 所属岗位
        /// </summary>
        public List<Guid> MemberedPosts { get; set; }

        public CreateOrUpdateUserInput()
        {
            OrganizationUnits = new List<Guid>();
        }
    }
}