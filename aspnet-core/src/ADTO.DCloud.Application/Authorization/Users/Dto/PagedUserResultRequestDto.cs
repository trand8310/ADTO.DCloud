using ADTOSharp.Application.Services.Dto;
using System;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    //custom PagedResultRequestDto
    public class PagedUserResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
