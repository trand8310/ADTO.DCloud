using System;
using System.ComponentModel;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.Customers.Dto
{
    /// <summary>
    /// 客户相关产品
    /// </summary>
    [AutoMap(typeof(CustomerProduct))]
    public class CustomerProductDto : EntityDto<Guid?>
    {
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [Description("产品名称")]
        public string ProductName { get; set; }

        /// <summary>
        /// 产品规格
        /// </summary>
        [Description("产品规格")]
        public string Spec { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 年销售额
        /// </summary>
        [Description("年销售额")]
        public decimal SalesVolume { get; set; }

    }
}
