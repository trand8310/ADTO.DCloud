using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Runtime.Validation;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 员工列表请求参数
    /// </summary>
    public class GetEmployeePagedInput :PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// 用户入职状态-审批状态
        /// </summary>
        public bool? AccountApprovalStatus { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public Guid? CompanyId { get; set; }
        /// <summary>
        /// 性别
        /// </summary>	
        public string Gender { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        public string PoliticalOutlook { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? InJobDate { get; set; }
        /// <summary>
        /// 离职日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? OutJobDate { get; set; }
        /// <summary>
        /// 转正日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? RegularDate { get; set; }
        /// <summary>
        /// 人员类型
        /// </summary>		
        public int? EmployeeType { get; set; }
        /// <summary>
        /// 办公地点
        /// </summary>
        public Guid? OfficeLocation { get; set; }
        /// <summary>
        /// 是否考勤（0：双休打卡/1：单休打卡/2：排班打卡/3：不打卡）
        /// </summary>	
        public string IsAttType { get; set; }
        /// <summary>
        /// （考勤时间）-对应之前的考勤区域
        /// </summary>	
        public Guid? AttTimeRuleId { get; set; }

        /// <summary>
        /// 直属上级
        /// </summary>
        public Guid? ManagerId { get; set; }
        /// <summary>
        /// 合同主体公司
        /// </summary>
        public string ContractCompanyId { get; set; }
        /// <summary>
        /// 所属事业部
        /// </summary>
        public string DivisionCompany { get; set; }
        /// <summary>
        /// 上级部门
        /// </summary>
        public string SuperiorDepartmentId { get; set; }
        /// <summary>
        /// 业务小组
        /// </summary>
        public string BusinessGroup { get; set; }
        /// <summary>
        /// 职务
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 职级
        /// </summary>
        public string PostLevelId { get; set; }

        public string Filter { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = " CreationTime desc";
            }

            Filter = Filter?.Trim();
        }
    }
}
