using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.CompanyPhones.Dto
{
    /// <summary>
    /// 公司号码
    /// </summary>
    [AutoMap(typeof(CompanyPhone))]
    public class CompanyPhoneDto: FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 所属用户
        /// </summary>
        public virtual Guid EmployeeId { get; set; }

        /// <summary>
        /// 所属用户
        /// </summary> 
        public EmployeeInfo Employee { get; set; }

        /// <summary>
        /// 办理日期
        /// </summary>
        public DateTime? ProcessingDate { get; set; }

        /// <summary>
        /// 公司号码
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Telephone { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string LastModifierUserName { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorUserName { get; set; }
    }
}
