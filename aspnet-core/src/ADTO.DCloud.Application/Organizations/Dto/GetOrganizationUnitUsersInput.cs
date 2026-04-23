using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Runtime.Validation;
using System;

namespace ADTO.DCloud.Organizations.Dto
{
    public class GetOrganizationUnitUsersInput : PagedAndSortedInputDto, IShouldNormalize
    {
 
        public Guid Id { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "user.Name";
            }

            Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
            {
                if (s.Contains("userName"))
                {
                    s = s.Replace("userName", "user.userName");
                }

                if (s.Contains("addedTime"))
                {
                    s = s.Replace("addedTime", "ouUser.creationTime");
                }

                return s;
            });
        }
    }
}
