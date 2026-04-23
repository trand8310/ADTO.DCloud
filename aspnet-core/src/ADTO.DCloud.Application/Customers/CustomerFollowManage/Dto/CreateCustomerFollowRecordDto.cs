using System;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Customers.CustomerFollowManage.Dto
{
    /// <summary>
    /// 新增客户跟进记录
    /// </summary>
    [AutoMapTo(typeof(CustomerFollowRecord))]
    public class CreateCustomerFollowRecordDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 所属客户Id
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 客户跟进方式 字典
        /// </summary>
        [Required(ErrorMessage ="跟进方式不能为空")]
        public string CustomerFollowType { get; set; }

        /// <summary>
        /// 客户跟进阶段 字段
        /// </summary>
        [Required(ErrorMessage = "跟进阶段不能为空")]
        public string CustomerFollowStage { get; set; }

        /// <summary>
        /// 跟进时间
        /// </summary>
        [Required(ErrorMessage = "跟进时间不能为空")]
        public DateTime FollowTime { get; set; }

        /// <summary>
        /// 跟进内容
        /// </summary>
        [Required(ErrorMessage = "跟进内容不能为空")]
        public string Content { get; set; }

        /// <summary>
        /// 跟进人Id
        /// </summary>
        [Required(ErrorMessage = "跟进人不能为空")]
        public string FollowUserId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
