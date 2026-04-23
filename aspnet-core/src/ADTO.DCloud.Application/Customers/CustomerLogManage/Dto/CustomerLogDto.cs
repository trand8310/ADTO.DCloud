using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Customers.CustomerLogManage.Dto
{
    /// <summary>
    /// 客户日志
    /// </summary>
    [AutoMap(typeof(CustomerLogs))]
    public class CustomerLogDto
    {
        /// <summary>
        /// 所属客户
        /// </summary>
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// 操作类型 字典（新增、修改、分享、审核、跟进 等所有操作）
        /// </summary>
        public string OperateType { get; set; }

        /// <summary>
        /// 数据详情
        /// </summary>
        public string DataDetail { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreatorUserId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
