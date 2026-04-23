
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.DynamicEntityProperties;
using System;

namespace ADTO.DCloud.DynamicEntityProperties.Dto;

[AutoMap(typeof(DynamicPropertyValue))]
public class DynamicPropertyValueDto : EntityDto<Guid>
{
    public virtual string Value { get; set; }

    public Guid? TenantId { get; set; }

    public Guid DynamicPropertyId { get; set; }
}
