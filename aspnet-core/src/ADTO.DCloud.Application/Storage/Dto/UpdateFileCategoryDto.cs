

using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;

namespace ADTO.DCloud.Storage.Dto
{
    /// <summary>
    /// UpdateFileCategoryDto
    /// </summary>
    [AutoMapTo(typeof(SharedFileCategory))]
    public class UpdateFileCategoryDto : EntityDto<Guid>
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 父级类目
        /// </summary>
        public Guid? ParentCategoryId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SortCode { get; set; }

        /// <summary>
        /// 共享权限
        /// </summary>
        public List<SharedFileAuthorizeDto> Permissions { get; set; }
    }
}
