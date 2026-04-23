using ADTOSharp.Localization;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Localization.Dto
{
    public class CreateOrUpdateLanguageInput
    {
        [Required]
        public ApplicationLanguageEditDto Language { get; set; }

    }
}