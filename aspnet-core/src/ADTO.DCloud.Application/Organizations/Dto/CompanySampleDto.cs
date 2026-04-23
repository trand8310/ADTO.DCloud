using System;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization.Users;
using ADTOSharp.AutoMapper;
using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Organizations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTO.DCloud.Organizations.Dto;
using ADTO.DCloud.Authorization.Users.Dto;

namespace ADTO.DCloud.Authorization.Organizations.Dto
{
    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class CompanySampleDto : EntityDto<Guid>
    {
        /// <summary>
        /// 组织代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        public virtual UserLightDto ManagerUser { get; set; }
        public virtual DateTime CreationTime { get; set; }

    }
}
