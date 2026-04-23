

using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Extensions;

namespace ADTO.DCloud.Storage.Dto
{
    /// <summary>
    /// 按页获取文件目录
    /// </summary>
    public class PagedFileCategoryResultRequestDto : PagedAndSortedResultRequestDto
    {

        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }
        public void Normalize()
        {
            if (Sorting.IsNullOrWhiteSpace())
            {
                Sorting = " SortCode asc,CreationTime desc  ";
            }
            Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
            {
                return s;
            });
        }
    }
}
