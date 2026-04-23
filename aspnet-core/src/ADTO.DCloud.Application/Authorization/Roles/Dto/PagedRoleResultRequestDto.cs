using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.Authorization.Roles.Dto;

public class PagedRoleResultRequestDto : PagedResultRequestDto, IShouldNormalize
{
    public string Keyword { get; set; }
    public string Sorting { get; set; }

    public void Normalize()
    {
        if (string.IsNullOrEmpty(Sorting))
        {
            Sorting = "Name,DisplayName";
        }

        Keyword = Keyword?.Trim();
    }
}

