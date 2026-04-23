using ADTOSharp.Events.Bus;
using System;


namespace ADTO.DCloud.MultiTenancy
{
    public class TenantEditionChangedEventData : EventData
    {
        public Guid TenantId { get; set; }

        public Guid? OldEditionId { get; set; }

        public Guid? NewEditionId { get; set; }
    }
}