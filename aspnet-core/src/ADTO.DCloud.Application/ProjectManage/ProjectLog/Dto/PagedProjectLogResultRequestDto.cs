using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;

namespace ADTO.DCloud.ProjectManage.Dto
{
    public class PagedProjectLogResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public Guid ProjectId { get; set; }

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
                if (string.IsNullOrEmpty(Sorting))
                {
                    Sorting = "CreationTime ";
                }
            }
        }
    }
}
