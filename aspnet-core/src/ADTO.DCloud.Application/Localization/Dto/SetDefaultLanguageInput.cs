using ADTOSharp.Localization;
using System.ComponentModel.DataAnnotations;


namespace ADTO.DCloud.Localization.Dto
{
    public class SetDefaultLanguageInput
    {
        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public virtual string Name { get; set; }
    }
}