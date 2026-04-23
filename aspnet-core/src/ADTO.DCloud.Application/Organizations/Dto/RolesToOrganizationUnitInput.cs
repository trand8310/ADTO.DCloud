using System;

namespace ADTO.DCloud.Organizations.Dto
{
    public class RolesToOrganizationUnitInput
    {
        public Guid[] RoleIds { get; set; }

        public Guid OrganizationUnitId { get; set; }
    }
}