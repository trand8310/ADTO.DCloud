using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.Authorization.Users.Delegation.Dto
{
    public class GetUserDelegationsInput : IPagedResultRequest, ISortedResultRequest, IShouldNormalize
    {
        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public string Sorting { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting) || Sorting == "userName ASC")
            {
                Sorting = "Username";
            }

            Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
            {
                if (s == "userName DESC")
                {
                    s = "UserName DESC";
                }

                return s;
            });
        }
    }
}
