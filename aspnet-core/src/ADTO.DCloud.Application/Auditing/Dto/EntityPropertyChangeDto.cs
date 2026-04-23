using ADTOSharp.Application.Services.Dto;
using System;

namespace ADTO.DCloud.Auditing.Dto
{
    public class EntityPropertyChangeDto : EntityDto<long>
    {
        public long EntityChangeId { get; set; }

        public string NewValue { get; set; }

        public string OriginalValue { get; set; }

        public string PropertyName { get; set; }

        public string PropertyTypeFullName { get; set; }

        public Guid? TenantId { get; set; }
    }
}