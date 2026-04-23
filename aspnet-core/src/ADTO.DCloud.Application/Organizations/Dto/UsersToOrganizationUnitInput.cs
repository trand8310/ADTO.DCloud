using System;

namespace ADTO.DCloud.Organizations.Dto
{
    public class UsersToOrganizationUnitInput
    {
        public Guid[] UserIds { get; set; }

        public Guid OrganizationUnitId { get; set; }
    }
}