using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Runtime.Validation;


namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class GetLinkedUsersInput : IPagedResultRequest, ISortedResultRequest, IShouldNormalize
    {
        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public string Sorting { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting) || Sorting == "userName ASC")
            {
                Sorting = "TenancyName, Username";
            }

            Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
            {
                if (s == "userName DESC")
                {
                    s = "TenancyName DESC, UserName DESC";
                }

                return s;
            });
        }
    }
}
