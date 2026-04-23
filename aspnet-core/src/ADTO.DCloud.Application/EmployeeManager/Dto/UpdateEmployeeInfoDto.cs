using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Organizations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 修改员工基本信息Dto
    /// </summary>
    [AutoMapTo(typeof(EmployeeInfo))]
    public class UpdateEmployeeInfoDto : EntityDto<Guid>
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// 合同信息
        /// </summary>
        public EmployeeContractDto Contract { get; set; }
        /// <summary>
        /// 家庭信息
        /// </summary>
        public EmployeeFamilieDto Familie { get; set; }
        /// <summary>
        /// 直属上级
        /// </summary>
        public Guid? ManagerId { get; set; }
        ///// <summary>
        ///// 归属部门
        ///// </summary>
        public Guid? DepartmentId { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public Guid? CompanyId { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 员工登录密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 性别
        /// </summary>	
        public string Gender { get; set; }
        /// <summary>
        /// 职级
        /// </summary>
        public int? PostLevelId { get; set; }

        /// <summary>
        /// 日历
        /// </summary>
        public string Calendar { get; set; }
        /// <summary>
        /// 生日
        /// </summary>	
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [MaxLength(120)]
        public string IDCard { get; set; }
        /// <summary>
        /// QQ号
        /// </summary>	
        [MaxLength(120)]
        public string OICQ { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>	
        [MaxLength(120)]
        public string WeChat { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 办公地点
        /// </summary>
        public Guid? OfficeLocation { get; set; }
        /// <summary>
        /// 是否考勤（0：双休打卡/1：单休打卡/2：排班打卡/3：不打卡）
        /// </summary>	
        public int IsAttType { get; set; }
        /// <summary>
        /// 允许APP打卡
        /// </summary>	
        public int IsAPPAtt { get; set; }
        /// <summary>
        /// （考勤时间）-对应之前的考勤区域
        /// </summary>	
        public Guid? AttTimeRuleId { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime? InJobDate { get; set; }
        /// <summary>
        /// 离职日期
        /// </summary>
        public DateTime? OutJobDate { get; set; }
        /// <summary>
        /// 人员类型
        /// </summary>		
        public int? EmployeeType { get; set; }
        /// <summary>
        /// 职务
        /// </summary>
        [MaxLength(120)]
        public string Position { get; set; }
        /// <summary>
        /// 业务小组
        /// </summary>
        [MaxLength(120)]
        public string BusinessGroup { get; set; }
        /// <summary>
        /// 备注
        /// </summary>	
        [MaxLength(225)]
        public string Description { get; set; }
        /// <summary>
        /// 允许用户打卡规则，钉钉打卡限制时间，钉钉签到不要求时间，上午下午各一条记录
        /// </summary>
        public int CheckInRules { get; set; }
        /// <summary>
        /// 考勤规则生效日期
        /// </summary>
        public DateTime? CheckInRulesEffectiveDate { get; set; }
        /// <summary>
        /// 用户入职审批状态
        /// </summary>
        public bool AccountApprovalStatus { get; set; }
        /// <summary>
        /// 用户入职状态
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 上级部门
        /// </summary>
        [MaxLength(120)]
        public string SuperiorDepartmentId { get; set; }
        /// <summary>
        /// 所属事业部公司
        /// </summary>
        [MaxLength(120)]
        public string DivisionCompany { get; set; }
    }
}
