using System.Globalization;
using System.Linq;
using System.Text;
using ADTOSharp.Dependency;
using ADTOSharp.Json;
using ADTOSharp.Localization;
using ADTOSharp.Web.Http;

namespace ADTOSharp.Web.Localization
{
    internal class LocalizationScriptManager : ILocalizationScriptManager, ISingletonDependency
    {
        private readonly ILocalizationManager _localizationManager;
        private readonly ILanguageManager _languageManager;

        public LocalizationScriptManager(
            ILocalizationManager localizationManager,
            ILanguageManager languageManager)
        {
            _localizationManager = localizationManager;
            _languageManager = languageManager;
        }

        /// <inheritdoc/>
        public string GetScript()
        {
            return GetScript(CultureInfo.CurrentUICulture);
        }

        /// <inheritdoc/>
        public string GetScript(CultureInfo cultureInfo)
        {
            //NOTE: Disabled caching since it's not true (localization script is changed per user, per tenant, per culture...)
            return BuildAll(cultureInfo);
            //return _cacheManager.GetCache(ADTOSharpCacheNames.LocalizationScripts).Get(cultureInfo.Name, () => BuildAll(cultureInfo));
        }

        private string BuildAll(CultureInfo cultureInfo)
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();
            script.AppendLine("    adto.localization = adto.localization || {};");
            script.AppendLine();
            script.AppendLine("    adto.localization.currentCulture = {");
            script.AppendLine("        name: '" + HttpEncode.JavaScriptStringEncode(cultureInfo.Name) + "',");
            script.AppendLine("        displayName: '" + HttpEncode.JavaScriptStringEncode(cultureInfo.DisplayName) + "'");
            script.AppendLine("    };");
            script.AppendLine();
            script.Append("    adto.localization.languages = [");

            var languages = _languageManager.GetActiveLanguages();
            for (var i = 0; i < languages.Count; i++)
            {
                var language = languages[i];

                script.AppendLine("{");
                script.AppendLine("        name: '" + HttpEncode.JavaScriptStringEncode(language.Name) + "',");
                script.AppendLine("        displayName: '" + HttpEncode.JavaScriptStringEncode(language.DisplayName) + "',");
                script.AppendLine("        icon: '" + HttpEncode.JavaScriptStringEncode(language.Icon) + "',");
                script.AppendLine("        isDisabled: " + language.IsDisabled.ToString().ToLowerInvariant() + ",");
                script.AppendLine("        isDefault: " + language.IsDefault.ToString().ToLowerInvariant());
                script.Append("    }");

                if (i < languages.Count - 1)
                {
                    script.Append(" , ");
                }
            }

            script.AppendLine("];");
            script.AppendLine();

            if (languages.Count > 0)
            {
                var currentLanguage = _languageManager.CurrentLanguage;
                script.AppendLine("    adto.localization.currentLanguage = {");
                script.AppendLine("        name: '" + HttpEncode.JavaScriptStringEncode(currentLanguage.Name) + "',");
                script.AppendLine("        displayName: '" + HttpEncode.JavaScriptStringEncode(currentLanguage.DisplayName) + "',");
                script.AppendLine("        icon: '" + HttpEncode.JavaScriptStringEncode(currentLanguage.Icon) + "',");
                script.AppendLine("        isDisabled: " + currentLanguage.IsDisabled.ToString().ToLowerInvariant() + ",");
                script.AppendLine("        isDefault: " + currentLanguage.IsDefault.ToString().ToLowerInvariant());
                script.AppendLine("    };");
            }

            var sources = _localizationManager.GetAllSources().OrderBy(s => s.Name).ToArray();

            script.AppendLine();
            script.AppendLine("    adto.localization.sources = [");

            for (int i = 0; i < sources.Length; i++)
            {
                var source = sources[i];
                script.AppendLine("        {");
                script.AppendLine("            name: '" + HttpEncode.JavaScriptStringEncode(source.Name) + "',");
                script.AppendLine("            type: '" + source.GetType().Name + "'");
                script.AppendLine("        }" + (i < (sources.Length - 1) ? "," : ""));
            }

            script.AppendLine("    ];");

            script.AppendLine();
            script.AppendLine("    adto.localization.values = adto.localization.values || {};");
            script.AppendLine();

            foreach (var source in sources)
            {
                script.Append("    adto.localization.values['" + HttpEncode.JavaScriptStringEncode(source.Name) + "'] = ");

                var stringValues = source.GetAllStrings(cultureInfo).OrderBy(s => s.Name).ToList();
                var stringJson = stringValues
                    .ToDictionary(_ => _.Name, _ => _.Value)
                    .ToJsonString(indented: true);
                script.Append(stringJson);

                script.AppendLine(";");
                script.AppendLine();
            }

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}
