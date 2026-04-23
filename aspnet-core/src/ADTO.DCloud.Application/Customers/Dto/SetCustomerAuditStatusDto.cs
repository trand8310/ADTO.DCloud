using System;
 
namespace ADTO.DCloud.Customers.Dto
{
    /// <summary>
    /// 客户审核
    /// </summary>
    public class SetCustomerAuditStatusDto
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public bool AuditStatus { get; set; }
    }
}
