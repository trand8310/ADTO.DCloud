using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;

namespace ADTO.DCloud.Media.UploadFiles.Dto
{
    /// <summary>
    /// 文件分页查询
    /// </summary>
    public class PagedFileResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 具体业务对象Id（存具体关联对象Id，例如客户Id、项目Id）
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 所属文件分类Id
        /// </summary>
        public Guid? FileTypeId { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
