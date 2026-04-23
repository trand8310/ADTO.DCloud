using ADTOSharp.Localization;
using System.ComponentModel.DataAnnotations;


namespace ADTO.DCloud.Localization.Dto
{
    public class DeleteLanguageTextInput
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public string LanguageName { get; set; }
        /// <summary>
        /// 来源,删除只能删除DCloud来源的KEY值
        /// </summary>
        [Required]
        [StringLength(ApplicationLanguageText.MaxSourceNameLength)]
        public string SourceName { get; set; }

        /// <summary>
        /// 文本KEY值
        /// </summary>
        [Required]
        [StringLength(ApplicationLanguageText.MaxKeyLength)]
        public string Key { get; set; }
    }
}