using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm
{
    /// <summary>
    /// 考勤异常
    /// </summary>
    [Description("考勤异常"), Table("Adto_Att")]
    public class Adto_Att : FullAuditedEntity<Guid>, IRemark
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
        /// 异常时间
        /// </summary>
        public DateTime AttDate { get; set; }

        /// <summary>
        /// 异常类型
        /// </summary>
        [StringLength(100)]
        public string AttType { get; set; }

        /// <summary>
        /// 异常原因
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 附件证明
        /// </summary>
        public string Attachment { get; set; }

    }
}
