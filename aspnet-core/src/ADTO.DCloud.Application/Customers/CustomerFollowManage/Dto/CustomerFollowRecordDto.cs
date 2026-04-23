using System;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.Customers.CustomerFollowManage.Dto
{
    /// <summary>
    /// 客户跟进记录
    /// </summary>
    [AutoMap(typeof(CustomerFollowRecord))]
    public class CustomerFollowRecordDto : EntityDto<Guid>
    {
        /// <summary>
        /// 所属客户Id
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
        /// 客户跟进方式 字典
        /// </summary>
        public string CustomerFollowType { get; set; }

        /// <summary>
        /// 客户跟进阶段 字段
        /// </summary>
        public string CustomerFollowStage { get; set; }

        /// <summary>
        /// 跟进时间
        /// </summary>
        public DateTime FollowTime { get; set; }

        /// <summary>
        /// 跟进内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 跟进人Id
        /// </summary>
        public Guid FollowUserId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 跟进人图像
        /// </summary>
        public string FollowProfilePicture { get; set; }

        /// <summary>
        /// 跟进人名称
        /// </summary>
        public string FollowUserName { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreatorUserId { get; set; }

    }
}
