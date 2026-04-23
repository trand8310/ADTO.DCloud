using System;

namespace ADTO.DCloud.Organizations.Dto
{
    public class UserToOrganizationUnitInput
    {
        public Guid UserId { get; set; }
        public Guid OrganizationUnitId { get; set; }
    }
}