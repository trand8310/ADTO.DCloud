

using ADTO.OA.Storage.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;

namespace ADTO.DCloud.Storage.Dto
{
    /// <summary>
    /// 文件目录
    /// </summary>
    [AutoMap(typeof(SharedFileCategory))]
    public class FileCategoryDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 目录名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 父级目录
        /// </summary>
        public Guid? ParentCategoryId { get; set; }
        /// <summary>
        /// 父级目录
        /// </summary>
        public FileCategoryDto? Parent { get; set; }

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
