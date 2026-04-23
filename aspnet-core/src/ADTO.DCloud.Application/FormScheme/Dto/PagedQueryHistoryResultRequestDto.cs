using System;
using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.FormScheme.Dto
{
    /// <summary>
    /// 查询表单历史记录分页列表
    /// </summary>
    public class PagedQueryHistoryResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 模板信息主键
        /// </summary>
        public Guid SchemeInfoId { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
