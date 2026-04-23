using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.AutoMapper;
using ADTOSharp.Runtime.Validation;
using System;


namespace ADTO.DCloud.MultiTenancy.Dto;

[AutoMapFrom(typeof(Tenant))]
public class GetTenantsDto : PagedAndSortedInputDto, IShouldNormalize
{
    /// <summary>
    /// 关键词搜索
    /// </summary>
    public string Keyword { get; set; }
    public DateTime? SubscriptionEndDateStart { get; set; }
    public DateTime? SubscriptionEndDateEnd { get; set; }
    public DateTime? CreationDateStart { get; set; }
    public DateTime? CreationDateEnd { get; set; }
    public Guid? EditionId { get; set; }
    public bool EditionIdSpecified { get; set; }

    public void Normalize()
    {
        if (string.IsNullOrEmpty(Sorting))
        {
            Sorting = "TenancyName";
        }

        Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
        {
            return s.Replace("editionDisplayName", "Edition.DisplayName");
        });
    }
}

