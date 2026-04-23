using System;
using System.Collections.Generic;
using ADTO.DCloud.Organizations.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Organizations;
using ADTOSharp.Timing;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserListDto : EntityDto<Guid>, IPassivable, IHasCreationTime
    {
        public string Name { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>

        public string PhoneNumber { get; set; }
        /// <summary>
        /// 用户配置
        /// </summary>

        public Guid? ProfilePictureId { get; set; }

        /// <summary>
        /// 邮箱是否确认
        /// </summary>

        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        public List<UserListRoleDto> Roles { get; set; }

        /// <summary>
        /// 部门,默认部门,在维护用户的部门时,默认第一条记录为默认部门,可以修改任一记录为默认部门
        /// </summary>
        public Guid? DepartmentId { get; set; }

        public OrganizationUnitSampleDto? Department { get; set; }
        /// <summary>
        /// 用户组织
        /// </summary>
        public List<UserListOrganizationUnitDto> OrganizationUnits { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public Guid? CompanyId { get; set; }
        public OrganizationUnitSampleDto? Company { get; set; }

        /// <summary>
        /// 直属上级
        /// </summary>
        public Guid? ManagerId { get; set; }
        /// <summary>
        /// 激活状态
        /// </summary>
        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }
    }
}