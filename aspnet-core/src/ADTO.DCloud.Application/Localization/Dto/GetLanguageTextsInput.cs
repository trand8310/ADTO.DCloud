using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Localization;
using ADTOSharp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Extensions;



namespace ADTO.DCloud.Localization.Dto
{
    public class GetLanguageTextsInput : IPagedResultRequest, ISortedResultRequest, IShouldNormalize
    {
        [Range(0, int.MaxValue)]
        public int PageSize { get; set; } //0: Unlimited.

        [Range(0, int.MaxValue)]
        public int PageNumber { get; set; }

        public string Sorting { get; set; }

        /// <summary>
        /// ADTOSharp,ADTOSharpWeb,ADTOSharpZero,DCloud
        /// </summary>
        [Required]
        [MaxLength(ApplicationLanguageText.MaxSourceNameLength)]
        public string SourceName { get; set; }

        [StringLength(ApplicationLanguage.MaxNameLength)]
        public string BaseLanguageName { get; set; }
 

        //[Required]
        //[StringLength(ApplicationLanguage.MaxNameLength, MinimumLength = 2)]
        public string TargetLanguageName { get; set; }

        public string TargetValueFilter { get; set; }

        public string FilterText { get; set; }
        
        public void Normalize()
        {
            if (TargetValueFilter.IsNullOrEmpty())
            {
                TargetValueFilter = "ALL";
            }
        }
    }
}
