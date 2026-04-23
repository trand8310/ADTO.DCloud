using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTO.DCloud.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using ADTO.DCloud.Authorization.Posts.Dto;
using System.Collections.Generic;

namespace ADTO.DCloud.Sessions.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserLoginInfoDto : EntityDto<Guid>
    {
        UserLoginInfoDto() { PostList = new List<PostDto>(); }
        public string Name { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public Guid? DepartmentId { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// 用户图像
        /// </summary>
        public string ProfilePicture { get; set; }

        /// <summary>
        /// 用户部门集合
        /// </summary>
        public List<PostDto> PostList { get; set; }

        ///// <summary>
        ///// 默认部门名称
        ///// </summary>
        //public string DepartmentName { get; set; }

        ///// <summary>
        ///// 默认公司名称
        ///// </summary>
        //public string CompanyName { get; set; }
    }
}
