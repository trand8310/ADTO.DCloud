using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 流程监控查询字段
    /// </summary>
    public class GetMonitorPageInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 状态 1运行2草稿3作废4结束5错误（异常）
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 离职 1 ，其它不是
        /// </summary>
        public int IsLeave { get; set; }
        /// <summary>
        /// 查询开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 查询结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 模糊查询（流程模版编码，流程标题）
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 发起人Id
        /// </summary>
        public Guid? CreateUserId { get; set; }
        /// <summary>
        /// 审批人Id
        /// </summary>
        public Guid? AuditUserId { get; set; }
        /// <summary>
        /// 流程类型
        /// </summary>
        public string SchemeCode { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime";
            }
            Keyword = Keyword?.Trim();
        }
    }
}
