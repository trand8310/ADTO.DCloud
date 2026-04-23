using System;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Customers.CustomerLogManage.Dto
{
    /// <summary>
    /// 创建客户操作日志
    /// </summary>
    [AutoMapTo(typeof(CustomerLogs))]
    public class CreateCustomerLogDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 所属客户
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 操作类型（新增、修改、分享、审核、跟进 等所有操作）
        /// </summary>
        public string OperateType { get; set; }

        /// <summary>
        /// 数据详情
        /// </summary>
        public string DataDetail { get; set; }

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
