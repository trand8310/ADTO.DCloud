using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.DatabaseManager.ImportTableSources.Dto
{
    /// <summary>
    /// 导入表源表、分页查询条件
    /// </summary>
    public class PagedImportTableSourcesResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
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
