using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    [AutoMap(typeof(EmployeeInfo))]
    public class GetUserIsActiveDto
    {
        /// <summary>
        /// 工号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 是否考勤（0：双休打卡/1：固定打卡/2：单休打卡/3：排班打卡，4：大小周，5：不打卡）
        /// </summary>	
        public string IsAttType { get; set; }

        public int AreaId { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public Guid? DepartmentId { get; set; }
        public string Department { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public Guid? CompanyId { get; set; }
        public string CompanyName { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime? InJobDate { get; set; }
        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime? OutJobDate { get; set; }
        public DateTime CreationTime { get; set; }

        public bool IsActive { get; set; }
        /// <summary>
        /// 办公地点
        /// </summary>
        public Guid? OfficeLocation { get; set; }

        /// <summary>
        /// （考勤时间）-对应之前的考勤区域
        /// </summary>	
        public Guid? AttTimeRuleId { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 允许用户打卡规则，钉钉打卡限制时间，钉钉签到不要求时间，上午下午各一条记录
        /// 0:考勤机,1:钉钉签到或考勤机,2:钉钉打卡或考勤机
        /// </summary>
        public string CheckInRules { get; set; }
        /// <summary>
        /// 打卡规则生效日期
        /// </summary>
        public DateTime? CheckInRulesEffectiveDate { get; set; }
    }
}
