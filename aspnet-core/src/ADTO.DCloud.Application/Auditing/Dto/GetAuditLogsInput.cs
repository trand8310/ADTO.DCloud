using System;
using ADTOSharp.Extensions;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Auditing.Dto
{
    public class GetAuditLogsInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 开始日期,必传
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 截止日期,必传
        /// </summary>
        [Required]

        public DateTime EndDate { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 服务类名
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }
        /// <summary>
        /// 是否存在异常
        /// </summary>
        public bool? HasException { get; set; }
        /// <summary>
        /// 最短执行时长
        /// </summary>
        public int? MinExecutionDuration { get; set; }
        /// <summary>
        /// 最长执行时长
        /// </summary>
        public int? MaxExecutionDuration { get; set; }

        public void Normalize()
        {
            if (Sorting.IsNullOrWhiteSpace())
            {
                Sorting = "ExecutionTime DESC";
            }

            Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
            {
	            if (s.IndexOf("UserName", StringComparison.OrdinalIgnoreCase) >= 0)
	            {
		            s = "User." + s;
	            }
	            else
	            {
		            s = "AuditLog." + s;
	            }

	            return s;
            });
        }
    }
}
