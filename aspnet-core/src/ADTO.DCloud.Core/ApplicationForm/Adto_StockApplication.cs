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
    /// 物品申请表包括（电脑、手机、总裁办工资及总裁办其他办公用品申请）
    /// </summary>
    [Table("Adto_StockApplications")]
    public class Adto_StockApplication : FullAuditedEntity<Guid>, IRemark
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
        /// 申请类型
        /// </summary>
        [StringLength(100)]
        public string ApplyType { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(225)]
        public string Title { get; set; }

        /// <summary>
        /// 理由
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
         
    }
}
