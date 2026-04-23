using System;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Media.UploadFileTypes.Dto
{
    /// <summary>
    /// 添加所属文件夹类别
    /// </summary>
    [AutoMapTo(typeof(UploadFileType))]
    public class CreateUploadFileTypeDto : EntityDto<Guid?>
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
    }
}
