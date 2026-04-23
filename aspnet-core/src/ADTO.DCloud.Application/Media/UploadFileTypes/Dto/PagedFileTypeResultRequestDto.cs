using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;

namespace ADTO.DCloud.Media.UploadFileTypes.Dto
{
    public class PagedFileTypeResultRequestDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 是否显示全部
        /// </summary>
        public bool IsAll { get; set; } = true;

        /// <summary>
        /// 表实体名称(类名区分是项目、客户。。。)
        /// </summary>
        public string ProjectClassName { get; set; }

        /// <summary>
        /// 实体Id(具体项目Id,客户Id)
        /// </summary>
        public Guid? ProjectId { get; set; }
    }
}
