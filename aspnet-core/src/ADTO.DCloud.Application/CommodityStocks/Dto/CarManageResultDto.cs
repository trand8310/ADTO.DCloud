
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 库存车辆 管理
    /// </summary>
    [AutoMap(typeof(CommodityStock))]
    public class CarManageResultDto : EntityDto<Guid>
    {
        /// <summary>
        /// 库存名称
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Name { get; set; }
        /// <summary>
        /// 车辆类别名称
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 车辆类别
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 车辆型号
        /// </summary>
        [StringLength(128)]
        public string Model { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        [StringLength(128)]
        public string SN { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SortCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 车辆状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 车辆名称-型号-加车牌号
        /// </summary>
        public string NameAndModelAndSn { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        [StringLength(225)]
        public string Description { get; set; }
    }
}
