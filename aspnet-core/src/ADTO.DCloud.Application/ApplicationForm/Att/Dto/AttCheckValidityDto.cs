using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Att.Dto
{

    /// <summary>
    /// 校验考勤异常DTO
    /// </summary>
    public class AttCheckValidityDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 异常时间
        /// </summary>
        public DateTime? AttDate { get; set; }

        /// <summary>
        /// 编辑表单Id
        /// </summary>
        public Guid Id { get; set; }
    }
}
