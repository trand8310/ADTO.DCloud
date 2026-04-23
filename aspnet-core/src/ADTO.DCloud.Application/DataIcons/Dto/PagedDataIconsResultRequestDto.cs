using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Runtime.Validation;
using System;

namespace ADTO.DCloud.DataIcons.Dto
{
    /// <summary>
    /// 图标分页查询
    /// </summary>
    public class PagedDataIconsResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime";
            }
        }
    }
}
