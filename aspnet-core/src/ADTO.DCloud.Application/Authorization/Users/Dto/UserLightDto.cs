using System;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization.Users;
using ADTOSharp.AutoMapper;
using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Organizations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTO.DCloud.Organizations.Dto;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserLightDto : EntityDto<Guid>
    {
       
        public string UserName { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 部门,默认部门,在维护用户的部门时,默认第一条记录为默认部门,可以修改任一记录为默认部门
        /// </summary>
        public virtual Guid? DepartmentId { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public virtual Guid? CompanyId { get; set; }


    }
}
