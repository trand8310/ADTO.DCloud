using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.DatabaseManager.Dto
{
    /// <summary>
    /// 数据库链接分页列表查询条件
    /// </summary>
    public class PagedDataConnectionResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键词查询
        /// </summary>
        public string Keyword { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                if (string.IsNullOrEmpty(Sorting))
                {
                    Sorting = "CreationTime ";
                }
            }
        }
    }
}
