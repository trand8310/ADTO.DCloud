using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Organizations;
using System;

namespace ADTO.DCloud.Organizations.Dto
{
    /// <summary>
    /// 手机端组织架构组件返回数据
    /// </summary>
    [AutoMapFrom(typeof(OrganizationUnit))]
    public class OrganizationUnitAppDto : EntityDto<Guid>
    {
        /// <summary>
        /// 上级组织
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 部门人数
        /// </summary>
        public int Employees { get; set; }

        /// <summary>
        /// 是否存在子级
        /// </summary>
        public bool HasChild { get; set; }

    }
}
