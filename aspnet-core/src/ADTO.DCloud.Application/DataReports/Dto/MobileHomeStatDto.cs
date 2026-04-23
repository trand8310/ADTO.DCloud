using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataReports.Dto
{
    /// <summary>
    /// 手机端首页统计Dto
    /// </summary>
    public class MobileHomeStatDto
    {
        /// <summary>
        /// 类目名称（客户、项目、跟进记录、合同签约）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Nums { get; set; }

        /// <summary>
        /// 上月、上年度、上季度、昨天
        /// </summary>
        public string PreName { get; set; }

        /// <summary>
        /// 昨天、上月、上季度、上年度数量（根据日期类别）
        /// </summary>
        public int PreNum { get; set; }

        /// <summary>
        /// 上浮比
        /// </summary>
        public float FloatRatio { get; set; }

        /// <summary>
        /// 图标地址
        /// </summary>
        public string IconUrl { get; set; }
    }
}
