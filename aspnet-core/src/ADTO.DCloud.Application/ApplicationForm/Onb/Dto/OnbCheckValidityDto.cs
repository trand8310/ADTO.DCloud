using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Onb.Dto
{
    /// <summary>
    /// 出差申请单校验DTO
    /// </summary>
    public class OnbCheckValidityDto
    {
        /// <summary>
        /// 申请人
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        public Guid? Id { get; set; }
    }
}
