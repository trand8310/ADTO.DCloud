

using ADTO.DCloud.Dto;
using System;

namespace ADTO.DCloud.Organizations.Dto
{
    public class FindOrganizationUnitRolesInput : PagedAndFilteredInputDto
    {
        public Guid OrganizationUnitId { get; set; }
    }
}