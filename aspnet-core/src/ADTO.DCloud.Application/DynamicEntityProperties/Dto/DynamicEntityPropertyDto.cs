
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.DynamicEntityProperties;
using System;

namespace ADTO.DCloud.DynamicEntityProperties.Dto;

[AutoMap(typeof(DynamicEntityProperty))]
public class DynamicEntityPropertyDto : EntityDto<Guid>
{
    /// <summary>
    /// 实体名称(含命名空间)
    /// </summary>
    public string EntityFullName { get; set; }
    /// <summary>
    /// 属性名称
    /// </summary>

    public string DynamicPropertyName { get; set; }
    /// <summary>
    /// 属性Id
    /// </summary>

    public Guid DynamicPropertyId { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    public Guid? TenantId { get; set; }
}