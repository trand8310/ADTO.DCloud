using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Out.Dto
{
    public class OutCheckValidityDto
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
