using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Runtime.Validation;
using System;

namespace ADTO.DCloud.Organizations.Dto
{
    public class GetOrganizationUnitRolesInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public Guid Id { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "role.DisplayName, role.Name";
            }

            Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
            {
                if (s.Contains("displayName"))
                {
                    s = s.Replace("displayName", "role.displayName");
                }

                if (s.Contains("addedTime"))
                {
                    s = s.Replace("addedTime", "ouRole.creationTime");
                }

                return s;
            });
        }
    }
}
