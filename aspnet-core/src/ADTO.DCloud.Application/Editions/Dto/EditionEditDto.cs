using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Editions.Dto
{
    public class EditionEditDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public Guid? ExpiringEditionId { get; set; }
    }
}