using System;
using ADTOSharp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.Customers.CustomerShareManage.Dto
{
    /// <summary>
    /// 添加客户分享
    /// </summary>
    [AutoMapTo(typeof(CustomerShareRecord))]
    public class CreateCustomerShareRecordDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 所属客户
        /// </summary>
        [Required(ErrorMessage ="所属客户不能为空")]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 分享人
        /// </summary>
        [Required(ErrorMessage = "分享人不能为空")]
        public Guid FromUserId { get; set; }

        /// <summary>
        /// 被分享人,多个
        /// </summary>
        public List<Guid> ToUserIdList { get; set; }

        ///// <summary>
        ///// 是否可用
        ///// </summary>
        //public bool IsActive { get; set; }
    }

}
