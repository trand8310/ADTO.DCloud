using System;

namespace ADTO.DCloud.Organizations.Dto
{
    public class MoveOrganizationUnitInput
    {
        public Guid Id { get; set; }

        public Guid? NewParentId { get; set; }
    }
}