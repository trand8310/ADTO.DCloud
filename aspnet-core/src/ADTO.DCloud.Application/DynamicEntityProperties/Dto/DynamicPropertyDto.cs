using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.DynamicEntityProperties;
using System;

namespace ADTO.DCloud.DynamicEntityProperties.Dto;

[AutoMap(typeof(DynamicProperty))]
public class DynamicPropertyDto : EntityDto<Guid>
{
    public string PropertyName { get; set; }

    public string DisplayName { get; set; }

    public string InputType { get; set; }

    public string Permission { get; set; }

    public Guid? TenantId { get; set; }
}
