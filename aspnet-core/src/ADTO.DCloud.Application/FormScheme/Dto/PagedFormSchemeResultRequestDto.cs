using ADTO.DCloud.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.FormScheme.Dto
{
    public class PagedFormSchemeResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public bool? IsActive { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime";
            }
        }
    }
}
