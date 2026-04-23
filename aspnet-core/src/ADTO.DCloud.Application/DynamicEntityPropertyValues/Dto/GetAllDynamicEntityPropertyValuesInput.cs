using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.DynamicEntityPropertyValues.Dto;

public class GetAllDynamicEntityPropertyValuesInput
{
    [Required]
    public string EntityFullName { get; set; }

    [Required]
    public string EntityId { get; set; }
}
