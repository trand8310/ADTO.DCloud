using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 员工合同信息
    /// </summary>
    [AutoMap(typeof(EmployeeContracts))]
    public class EmployeeContractDto : EntityDto<Guid>
    {
        ///// <summary>
        ///// 员工信息Id
        ///// </summary>
        //public Guid? EmployeeId { get; set; }
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
        public string ChangesStatus { get; set; }
        /// <summary>
        /// 推荐/招聘人
        /// </summary>
        public Guid? RecruitersId { get; set; }
        /// <summary>
        /// 推荐/招聘人
        /// </summary> 
        public string Recruiters { get; set; }
        /// <summary>
        /// 合同主体公司
        /// </summary>
        public string ContractCompany { get; set; }
        /// <summary>
        /// 合同主体公司Id
        /// </summary>
        public string ContractCompanyId { get; set; }
        /// <summary>
        /// 合同主体部门
        /// </summary>
        public string ContractDepartmentId { get; set; }
        /// <summary>
        /// 公司所在地
        /// </summary>
        [StringLength(128)]
        public string CompanyAddress { get; set; }

        /// <summary>
        /// 资料提交
        /// </summary>
        public string Attachment { get; set; }
        public List<UploadFilesInputDto> AttachmentList { get; set; }= [];
        /// <summary>
        /// 合同最后签订日期
        /// </summary>
        public DateTime? LastContractDate { get; set; }
    }
}
