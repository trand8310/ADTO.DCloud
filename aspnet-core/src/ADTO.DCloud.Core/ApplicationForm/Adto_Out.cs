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
    /// 外出申请
    /// </summary>
    [Description("出差申请"), Table("Adto_Out")]
    public class Adto_Out : FullAuditedEntity<Guid>, IRemark
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserID { get; set; }

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
        /// 外出原因
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
    }
}
