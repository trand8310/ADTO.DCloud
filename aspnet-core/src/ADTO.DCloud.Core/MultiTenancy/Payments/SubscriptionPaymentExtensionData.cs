using System;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;
using ADTOSharp.MultiTenancy;

namespace ADTO.DCloud.MultiTenancy.Payments
{
    [Table("SubscriptionPaymentsExtensionData")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class SubscriptionPaymentExtensionData : Entity<long>, ISoftDelete
    {
        public long SubscriptionPaymentId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public bool IsDeleted { get; set; }
    }
}
