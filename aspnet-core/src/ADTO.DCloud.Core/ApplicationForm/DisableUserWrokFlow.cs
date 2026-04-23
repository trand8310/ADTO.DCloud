using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm
{
    /// <summary>
    /// 申请单禁用用户（用户是否有被禁止申请出差或者是外出流程，钉钉打卡的用户）
    /// </summary>
    [Table("DisableUserWrokFlows")]
    public class DisableUserWrokFlow : CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// 申请单类型
        /// </summary>
        [Required]
        [StringLength(64)]
        public string ResourceTable { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
    }
}
