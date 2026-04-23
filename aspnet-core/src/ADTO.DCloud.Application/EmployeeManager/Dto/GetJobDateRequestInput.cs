using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    public class GetJobDateRequestInput : PagedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public long? DepartmentId { get; set; }

        /// <summary>
        /// 打卡类型--是否考勤（0：双休打卡/1：固定打卡/2：单休打卡/3：排班打卡，4：大小周，5：不打卡）
        /// </summary>
        public int? IsAttType { get; set; }
        /// <summary>
        /// 考勤区域
        /// </summary>
        public int? AreaId { get; set; }
        /// <summary>
        /// 办公地点
        /// </summary>
        public Guid? OfficeLocation { get; set; }

        /// <summary>
        /// （考勤时间）-对应之前的考勤区域
        /// </summary>	
        public Guid? AttTimeRuleId { get; set; }
        /// <summary>
        /// 是否分页
        /// </summary>
        public bool IsPage { get; set; }
    }
}
