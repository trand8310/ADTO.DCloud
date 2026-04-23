using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Events.Bus.Entities;
using System;
 

namespace ADTO.DCloud.Auditing.Dto
{
    public class EntityChangeDto:EntityDto<long>
    {
        public DateTime ChangeTime { get; set; }

        public EntityChangeType ChangeType { get; set; }

        public long EntityChangeSetId { get; set; }
        
        public string EntityId { get; set; }

        public string EntityTypeFullName { get; set; }

        public Guid? TenantId { get; set; }

        public object EntityEntry { get; set; }
    }
}