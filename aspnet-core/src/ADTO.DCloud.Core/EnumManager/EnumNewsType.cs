using ADTO.DCloud.EnumManager.EnumUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EnumManager
{
    /// <summary>
    /// 新闻枚举
    /// </summary>
    public enum EnumNewsType
    {
        /// <summary>
        /// 新闻
        /// </summary>
        [EnumDesc("新闻")]
        [Description("新闻")]
        News = 1,

        /// <summary>
        /// 公告
        /// </summary>
        [EnumDesc("公告")]
        [Description("公告")]
        Notice = 2,

        /// <summary>
        /// 钢材行情
        /// </summary>
        [EnumDesc("钢材行情")]
        [Description("钢材行情")]
        SteelMarket = 3,

    }
}
