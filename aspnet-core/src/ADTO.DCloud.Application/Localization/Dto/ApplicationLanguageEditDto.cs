using ADTOSharp.AutoMapper;
using ADTOSharp.Localization;
using System;
using System.ComponentModel.DataAnnotations;


namespace ADTO.DCloud.Localization.Dto
{

    [AutoMap(typeof(ApplicationLanguage))]
    public class ApplicationLanguageEditDto
    {
        public virtual Guid? Id { get; set; }

        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public virtual string Name { get; set; }

        [StringLength(ApplicationLanguage.MaxIconLength)]
        public virtual string Icon { get; set; }

        /// <summary>
        /// Mapped from Language.IsDisabled with using manual mapping in CustomDtoMapper.cs
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}