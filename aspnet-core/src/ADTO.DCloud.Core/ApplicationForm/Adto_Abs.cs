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
    /// 请假申请单
    /// </summary>
    [Table("Adto_Abs")]
    public class Adto_Abs : FullAuditedEntity<Guid>, IRemark
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
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 请假类型
        /// </summary>
        [StringLength(100)]
        public string AbsType { get; set; }

        /// <summary>
        /// 请假天数
        /// </summary>
        public decimal Days { get; set; }

        /// <summary>
        /// 请假原因
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 结婚证附件
        /// </summary>
        [StringLength(500)]
        public string MarriageAttach { get; set; }

        /// <summary>
        /// 调休备注
        /// </summary>
        [StringLength(500)]
        public string RestSummary { get; set; }
    }
}
