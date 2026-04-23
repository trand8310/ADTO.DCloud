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
    /// 制度发布申请单
    /// </summary>
    [Table("Adto_OaPolicyApproval")]
    public class Adto_OaPolicyApproval : FullAuditedEntity<Guid>, IRemark
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
        /// 制度名称
        /// </summary>
        [StringLength(225)]
        public string Name { get; set; }

        /// <summary>
        /// 文件编号
        /// </summary>
        [StringLength(225)]
        public string Code { get; set; }

        /// <summary>
        /// 文件层级
        /// </summary> 
        public int? Level { get; set; }
        /// <summary>
        /// 发布版本
        /// </summary>
        [StringLength(225)]
        public string Version { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 发布范围
        /// </summary>
        [StringLength(225)]
        public string Scope { get; set; }
        /// <summary>
        /// 是否工会审核
        /// </summary>
        [StringLength(500)]
        public int IsUnionApproval { get; set; }
        /// <summary>
        /// 工会红头文件
        /// </summary>
        [StringLength(225)]
        public string UnionAttachment { get; set; }
        /// <summary>
        /// 发布文件附件
        /// </summary>
        [StringLength(225)]
        public string PublishAttachment { get; set; }
        /// <summary>
        /// 商学院培训文件附件
        /// </summary>
        [StringLength(225)]
        public string CollegeAttachment { get; set; }

        /// <summary>
        /// 附件证明
        /// </summary>
        [StringLength(225)]
        public string Attachment { get; set; }
        /// <summary>
        /// 发布/修订
        /// 内容说明
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 核准签发节点审核人
        /// </summary>
        [StringLength(225)]
        public string ApprovalIssuer { get; set; }
    }
}
