using System;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System.Collections.Generic;

namespace ADTO.DCloud.Media.UploadFileTypes.Dto
{
    /// <summary>
    /// 所属文件夹类别
    /// </summary>
    [AutoMap(typeof(UploadFileType))]
    public class UploadFileTypeDto : FullAuditedEntityDto<Guid>, IDisplayOrder
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 表实体名称(类名区分是项目、客户。。。)
        /// </summary>
        public string ProjectClassName { get; set; }

        /// <summary>
        /// 实体Id(具体项目Id,客户Id)
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        public UploadFileTypeDto Parent { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        public List<UploadFileTypeDto>? Children { get; set; }
    }
}
