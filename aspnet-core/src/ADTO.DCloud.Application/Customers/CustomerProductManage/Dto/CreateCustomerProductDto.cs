using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Customers.CustomerProductManage.Dto
{
    /// <summary>
    /// 新增客户产品
    /// </summary>
    [AutoMapTo(typeof(CustomerProduct))]
    public class CreateCustomerProductDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 所属客户
        /// </summary>
        [Required(ErrorMessage = "所属客户不能为空")]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [Required(ErrorMessage = "产品名称不能为空")]
        public string ProductName { get; set; }

        /// <summary>
        /// 产品规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 年销售额
        /// </summary>
        public decimal SalesVolume { get; set; }

    }
}
