using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.FormScheme.Dto
{
    public enum ExecuteType
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        Insert = 0,
        /// <summary>
        /// 更新
        /// </summary>
        [Description("更新")]
        Update = 1,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 2
    }
}
