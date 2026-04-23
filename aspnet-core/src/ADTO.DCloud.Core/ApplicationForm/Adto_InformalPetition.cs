using ADTOSharp.Domain.Entities;
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
    /// 合同审批
    /// </summary>
    [Table("Adto_InformalPetition")]
    public class Adto_InformalPetition : FullAuditedEntity<Guid>, IRemark
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(225)]
        public string Title { get; set; }
        /// <summary>
        /// 合同名称
        /// </summary>
        [StringLength(200)]
        public string ContractName { get; set; }
        /// <summary>
        /// 合同金额
        /// </summary>
        [StringLength(100)]
        public string ContractAmount { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        [StringLength(100)]
        public string Commission { get; set; }
        /// <summary>
        /// 合同编号
        /// </summary>
        [StringLength(100)]
        public string ContractNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        [StringLength(500)]
        public string Files { get; set; }
    }
}
