

using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.DynamicEntityProperties;
using System;

namespace ADTO.DCloud.DynamicEntityProperties.Dto;

[AutoMap(typeof(DynamicEntityProperty))]
public class DynamicEntityPropertyValueDto : EntityDto<Guid>
{
    public string Value { get; set; }

    public string EntityId { get; set; }

    public Guid DynamicEntityPropertyId { get; set; }
}
