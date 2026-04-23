using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Abs.Dto
{
    /// <summary>
    /// 请假申请单校验DTO
    /// </summary>
    public class AbsCheckValidityDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 请假类型
        /// </summary>
        public int AbsType { get; set; }
        /// <summary>
        /// 请假天数
        /// </summary>
        public double Day { get; set; }
    }
}
