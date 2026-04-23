using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager
{
    /// <summary>
    /// 家庭信息
    /// </summary>
    [Description("员工家庭信息"), Table("EmployeeFamilies")]
    public class EmployeeFamilies : FullAuditedEntity<Guid>
    {
        ///// <summary>
        ///// 员工信息Id
        ///// </summary>
        //[Required]
        //public EmployeeInfo Employee { get; set; }
        //public virtual Guid EmployeeId { get; set; }
        /// <summary>
        /// 政治面貌
        /// </summary>
        [StringLength(128)]
        public string PoliticalOutlook { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        [StringLength(128)]
        public string Education { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        [StringLength(128)]
        public string Major { get; set; }
        /// <summary>
        /// 籍贯
        /// </summary>
        [StringLength(128)]
        public string NativePlace { get; set; }
        /// <summary>
        /// 家庭地址
        /// </summary>
        [StringLength(128)]
        public string HomeAddress { get; set; }
        /// <summary>
        /// 家庭电话
        /// </summary>
        [StringLength(128)]
        public string HomePhone { get; set; }
        /// <summary>
        /// 婚否
        /// </summary>
        public int? IsMarried { get; set; }

        /// <summary>
        /// 紧紧联系人
        /// </summary>
        [StringLength(128)]
        public string EmergencyContact { get; set; }
        /// <summary>
        /// 紧紧联系电话
        /// </summary>
        [StringLength(128)]
        public string EmergencyContactMobile { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string IdCardAttach { get; set; }
        /// <summary>
        /// 毕业证
        /// </summary>
        public string DiplomaAttach { get; set; }
        /// <summary>
        /// 学位证
        /// </summary>
        public string DegreeDiplomaAttach { get; set; }
        /// <summary>
        /// 离职证明
        /// </summary>
        public string LeavCertificateAttach { get; set; }
        /// <summary>
        /// 其他证
        /// </summary>
        public string CertificatesAttach { get; set; }

    }
}
