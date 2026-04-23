using System;

namespace ADTO.DCloud.Organizations.Dto
{
    public class RoleToOrganizationUnitInput
    {
        public Guid RoleId { get; set; }

        public Guid OrganizationUnitId { get; set; }
    }
}