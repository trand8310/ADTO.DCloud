using System;
using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 项目分页列表
    /// </summary>
    public class PagedProjectInfoRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 是否签订合同
        /// </summary>
        public bool? IsSign { get; set; }

        /// <summary>
        /// 项目金额
        /// </summary>
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// 项目金额
        /// </summary>
        public decimal? MaxAmount { get; set; }

        /// <summary>
        /// 创建开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
      
        /// <summary>
        /// 创建结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }

    }
}
