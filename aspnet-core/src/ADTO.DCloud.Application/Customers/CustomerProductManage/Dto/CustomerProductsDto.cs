using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Customers.CustomerProductManage.Dto
{
    /// <summary>
    /// 客户产品
    /// </summary>
    [AutoMap(typeof(CustomerProduct))]
    public class CustomerProductsDto:EntityDto<Guid>
    {
        /// <summary>
        /// 所属客户
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        public string CustomerCode { get; set; }

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
