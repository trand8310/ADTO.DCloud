using System;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.Customers.CustomerShareManage.Dto
{
    /// <summary>
    /// 客户分享记录
    /// </summary>
    [AutoMap(typeof(CustomerShareRecord))]
    public class CustomerShareRecordDto : EntityDto<Guid>
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
        /// 分享人
        /// </summary>
        public Guid FromUserId { get; set; }

        /// <summary>
        /// 分享人名称
        /// </summary>
        public string FromUserName { get; set; }

        /// <summary>
        /// 被分享人
        /// </summary>
        public Guid ToUserId { get; set; }

        /// <summary>
        /// 被分享人名称
        /// </summary>
        public string ToUserName { get; set; }

        ///// <summary>
        ///// 是否可用
        ///// </summary>
        //public bool IsActive { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid? CreatorUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
