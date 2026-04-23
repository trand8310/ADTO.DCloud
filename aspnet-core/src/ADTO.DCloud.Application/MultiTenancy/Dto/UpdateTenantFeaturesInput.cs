using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Application.Services.Dto;


namespace ADTO.DCloud.MultiTenancy.Dto;


public class UpdateTenantFeaturesInput
{
    [Range(1, int.MaxValue)]
    public Guid Id { get; set; }

    [Required]
    public List<NameValueDto> FeatureValues { get; set; }
}
