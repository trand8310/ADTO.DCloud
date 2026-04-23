
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
 

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 库存类别
    /// </summary>
    [AutoMap(typeof(CommodityStockCategory))]
    public class CreateCommodityStockCategoryDto:EntityDto<Guid?>
    {
        /// <summary>
        ///父级类别
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        [Required(ErrorMessage ="类别名称不能为空")]
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SortCode { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 库存类别用来区分是电脑库存还是总裁办库存-枚举（CommodityStockTypeEnum）
        /// </summary>
        public int Type { get; set; }
    }
}
