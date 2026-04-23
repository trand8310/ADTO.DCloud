using System.Collections.Generic;
using ADTOSharp.Localization;

namespace ADTOSharp.Web.Models.ADTOSharpUserConfiguration
{
    public class ADTOSharpUserLocalizationConfigDto
    {
        public ADTOSharpUserCurrentCultureConfigDto CurrentCulture { get; set; }

        public List<LanguageInfo> Languages { get; set; }

        public LanguageInfo CurrentLanguage { get; set; }

        public List<ADTOSharpLocalizationSourceDto> Sources { get; set; }

        public Dictionary<string, Dictionary<string, string>> Values { get; set; }
    }
}