using ADTO.DCloud.Editions.Dto;
using ADTOSharp.Application.Services.Dto;
using System.Collections.Generic;


namespace ADTO.DCloud.MultiTenancy.Dto;

public class GetTenantFeaturesEditOutput
{
    public List<NameValueDto> FeatureValues { get; set; }

    public List<FlatFeatureDto> Features { get; set; }
}
