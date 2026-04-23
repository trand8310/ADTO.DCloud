using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes.Dto
{
    /// <summary>
    /// 数据权限比较符
    /// </summary>
    public enum EnumConditionOperator
    {
        /// <summary>
        /// 等于（=）
        /// </summary>
        [Description("等于")]
        Equal=1,
        /// <summary>
        /// 大于（>）
        /// </summary>
        [Description("大于")]
        GreaterThan=2,
        /// <summary>
        /// 大于等于（>=）
        /// </summary>
        [Description("大于等于")]
        GreaterThanOrEqual=3,
        /// <summary>
        /// 小于（<）
        /// </summary>
        [Description("小于")]
        LessThan=4,
        /// <summary>
        /// 小于等于（<=）
        /// </summary>
        [Description("小于等于")]
        LessThanOrEqual=5,

        /// <summary>
        /// 包含（字符串包含或列表包含，如 LIKE 或 IN）
        /// </summary>
        [Description("包含")]
        Contains = 6,

        /// <summary>
        /// 包含于（值是否在指定列表中，如 IN）
        /// </summary>
        [Description("包含于")]
        In=7,

        /// <summary>
        /// 不等于（!= 或 <>）
        /// </summary>
        [Description("不等于")]
        NotEqual=8,

        /// <summary>
        /// 不包含（字符串不包含或列表不包含，如 NOT LIKE 或 NOT IN）
        /// </summary>
        [Description("不包含")]
        NotContains=9,      
        /// <summary>
        /// 不包含（字符串不包含或列表不包含，如 NOT LIKE 或 NOT IN）
        /// </summary>
        [Description("不包含于")]
        NotIn=10,
    }

}
