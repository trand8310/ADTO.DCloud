using ADTO.DCloud.EnumManager.EnumUtils;
using System.ComponentModel;

namespace ADTO.DCloud.CommodityStocks
{
    /// <summary>
    /// 库存枚举
    /// </summary>
    public enum EnumCommodityStockType
    {
        /// <summary>
        /// 电脑库存
        /// </summary>
        [EnumDesc("电脑库存")]
        [Description("电脑库存")]
        CommodityStocks = 1,

        /// <summary>
        /// 总裁办库存
        /// </summary>
        [EnumDesc("总裁办库存")]
        [Description("总裁办库存")]
        PresidentOfficeInventory = 2,
        /// <summary>
        /// 车辆类型
        /// </summary>
        [EnumDesc("车辆类型")]
        [Description("车辆类型")]
        CarManage = 3,
    }
}
