using ADTOSharp.Organizations;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Organizations.Dto
{
    public class CreateOrganizationUnitInput
    {
        public Guid? ParentId { get; set; }

        [Required]
        [StringLength(OrganizationUnit.MaxDisplayNameLength)]
        public string DisplayName { get; set; } 
    }
}