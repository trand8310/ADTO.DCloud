using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager
{
    /// <summary>
    /// 用户公司号码
    /// </summary>
    [Table("CompanyPhones")]
    public class CompanyPhone : FullAuditedEntity<Guid>, IPassivable
    {
        /// <summary>
        /// 所属用户
        /// </summary>
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// 所属用户
        /// </summary>
        [ForeignKey("EmployeeId")]
        public virtual EmployeeInfo Employee { get; set; }

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
    }
}
