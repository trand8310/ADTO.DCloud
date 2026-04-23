using System;
using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.ProjectManage.ProjectContracts.Dto
{
    /// <summary>
    /// 项目合同分页列表
    /// </summary>
    public class PagedProjectContractResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 签约开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 签约结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 合同金额
        /// </summary>
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// 合同金额
        /// </summary>
        public decimal? MaxAmount { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int? AuditStatus { get; set; }


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
