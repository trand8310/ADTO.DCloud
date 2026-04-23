using ADTO.DCloud.Infrastructure;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager
{
    /// <summary>
    /// 员工合同表
    /// </summary>
    [Description("员工合同表"), Table("EmployeeContracts")]
    public class EmployeeContracts : FullAuditedEntity<Guid>
    {
        ///// <summary>
        ///// 员工信息Id
        ///// </summary>
        //[Required]
        //public EmployeeInfo Employee { get; set; }
        /// <summary>
        /// 签到合同日期
        /// </summary>
        public DateTime? ContractDate { get; set; }
        /// <summary>
        /// 合同到期日期
        /// </summary>
        [StringLength(128)]
        public string ContractExpirationDate { get; set; }
        /// <summary>
        /// 合同签订次数
        /// </summary>
        public int? ContractSigningTimes { get; set; }
        /// <summary>
        /// 是否转正
        /// </summary>
        public int? IsRegular { get; set; }
        /// <summary>
        /// 转正情况
        /// </summary>
        [StringLength(128)]
        public string EmploymentStatus { get; set; }
        /// <summary>
        /// 转正日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? RegularDate { get; set; }
        /// <summary>
        /// 异动情况
        /// </summary>
        [StringLength(128)]
        public string ChangesStatus { get; set; }
        /// <summary>
        /// 推荐/招聘人
        /// </summary>
        [StringLength(128)]
        public Guid? RecruitersId { get; set; }
        /// <summary>
        /// 推荐/招聘人
        /// </summary>
        [StringLength(128)]
        public string Recruiters { get; set; }
        /// <summary>
        /// 合同主体公司
        /// </summary>
        [StringLength(128)]
        public string ContractCompany { get; set; }
        /// <summary>
        /// 合同主体公司Id
        /// </summary>
        [StringLength(128)]
        public string ContractCompanyId { get; set; }
        /// <summary>
        /// 合同主体部门
        /// </summary>
        [StringLength(128)]
        public string ContractDepartmentId { get; set; }
        /// <summary>
        /// 公司所在地
        /// </summary>
        [StringLength(128)]
        public string CompanyAddress { get; set; }
        /// <summary>
        /// 资料提交
        /// </summary>
        [StringLength(500)]
        public string Attachment { get; set; }
        /// <summary>
        /// 合同最后签订日期
        /// </summary>
        public DateTime? LastContractDate { get; set; }
    }
}
