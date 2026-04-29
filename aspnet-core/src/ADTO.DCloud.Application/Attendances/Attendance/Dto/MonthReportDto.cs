using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.Attendance.Dto
{
    /// <summary>
    /// 考勤月报表
    /// </summary>
    public class MonthReportDto
    {
        /// <summary>
        /// 工号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 是否（0：双休打卡/1：固定打卡/2：单休打卡/3：排班打卡，4：大小周，5：不打卡）---数据字典
        /// </summary>
        public int IsAttType { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 在职状态
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// 公司编号
        /// </summary>
        public Guid CompanyId { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 部门编号
        /// </summary>
        public Guid DepartmentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 工作时间
        /// </summary>
        public double WorkDays { get; set; }
        /// <summary>
        /// 正常天数
        /// </summary>
        public int NormalDays { get; set; }
        /// <summary>
        /// 病假
        /// </summary>
        public double BingJia { get; set; }
        public double ShiJia { get; set; }
        /// <summary>
        /// 产假
        /// </summary>
        public double ChanJia { get; set; }
        /// <summary>
        /// 丧假
        /// </summary>
        public double SangJia { get; set; }
        /// <summary>
        /// 婚假
        /// </summary>
        public double HunJia { get; set; }
        /// <summary>
        /// 年假
        /// </summary>
        public double NianJia { get; set; }
        /// <summary>
        /// 2022年度年假补充
        /// </summary>
        //public double NianJia2022 { get; set; }
        /// <summary>
        /// 调休
        /// </summary>
        public double TiaoXiu { get; set; }
        /// <summary>
        /// 出差
        /// </summary>
        public double ChuChai { get; set; }
        /// <summary>
        /// 新冠
        /// </summary>
        //public double XinGuan { get; set; }
        /// <summary>
        /// 考勤异常数
        /// </summary>
        public int Att { get; set; }
        /// <summary>
        /// 忘打卡
        /// </summary>
        public int ForgetPunchIn { get; set; }
        /// <summary>
        /// 迟到
        /// </summary>
        public int BeingLate { get; set; }
        /// <summary>
        /// 早退
        /// </summary>
        public int LeaveEarly { get; set; }
        /// <summary>
        /// 旷工
        /// </summary>
        public int Absenteeism { get; set; }

        /// <summary>
        /// 我需要审批流程-待审批数量
        /// </summary>
        public int UnfinishedTaskCount { get; set; }
    }
}
