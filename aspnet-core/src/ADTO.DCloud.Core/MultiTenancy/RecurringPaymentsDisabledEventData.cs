using ADTOSharp.Events.Bus;
using System;

namespace ADTO.DCloud.MultiTenancy
{
    public class RecurringPaymentsDisabledEventData : EventData
    {
        public Guid TenantId { get; set; }

        public Guid EditionId { get; set; }
    }
}
