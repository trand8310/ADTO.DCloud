using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Runtime.Session;

namespace ADTOSharp.Localization
{
    /// <summary>
    /// Implements <see cref="ILanguageProvider"/> to get languages from <see cref="IApplicationLanguageManager"/>.
    /// </summary>
    public class ApplicationLanguageProvider : ILanguageProvider
    {
        /// <summary>
        /// Reference to the session.
        /// </summary>
        public IADTOSharpSession ADTOSharpSession { get; set; }

        private readonly IApplicationLanguageManager _applicationLanguageManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApplicationLanguageProvider(IApplicationLanguageManager applicationLanguageManager)
        {
            _applicationLanguageManager = applicationLanguageManager;

            ADTOSharpSession = NullADTOSharpSession.Instance;
        }

        /// <summary>
        /// Gets the languages for current tenant.
        /// </summary>
        public async Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync()
        {
            var languageInfos = (await _applicationLanguageManager.GetLanguagesAsync(ADTOSharpSession.TenantId))
                    .OrderBy(l => l.DisplayName)
                    .Select(l => l.ToLanguageInfo())
                    .ToList();

            await SetDefaultLanguageAsync(languageInfos);

            return languageInfos;
        }

        /// <summary>
        /// Gets the languages for current tenant.
        /// </summary>
        public IReadOnlyList<LanguageInfo> GetLanguages()
        {
            var languageInfos = _applicationLanguageManager.GetLanguages(ADTOSharpSession.TenantId)
                    .OrderBy(l => l.DisplayName)
                    .Select(l => l.ToLanguageInfo())
                    .ToList();

            SetDefaultLanguage(languageInfos);

            return languageInfos;
        }
        /// <summary>
        /// Gets the active languages for current tenant.
        /// </summary>
        public IReadOnlyList<LanguageInfo> GetActiveLanguages()
        {
            var languageInfos = _applicationLanguageManager.GetActiveLanguages(ADTOSharpSession.TenantId)
               .OrderBy(l => l.DisplayName)
               .Select(l => l.ToLanguageInfo())
               .ToList();

            SetDefaultLanguage(languageInfos);

            return languageInfos;
        }

        private async Task SetDefaultLanguageAsync(List<LanguageInfo> languageInfos)
        {
            if (languageInfos.Count <= 0)
            {
                return;
            }

            var defaultLanguage = await _applicationLanguageManager.GetDefaultLanguageOrNullAsync(ADTOSharpSession.TenantId);
            if (defaultLanguage == null)
            {
                languageInfos[0].IsDefault = true;
                return;
            }

            var languageInfo = languageInfos.FirstOrDefault(l => l.Name == defaultLanguage.Name);
            if (languageInfo == null)
            {
                languageInfos[0].IsDefault = true;
                return;
            }

            languageInfo.IsDefault = true;
        }

        private void SetDefaultLanguage(List<LanguageInfo> languageInfos)
        {
            if (languageInfos.Count <= 0)
            {
                return;
            }

            var defaultLanguage = _applicationLanguageManager.GetDefaultLanguageOrNull(ADTOSharpSession.TenantId);
            if (defaultLanguage == null)
            {
                languageInfos[0].IsDefault = true;
                return;
            }

            var languageInfo = languageInfos.FirstOrDefault(l => l.Name == defaultLanguage.Name);
            if (languageInfo == null)
            {
                languageInfos[0].IsDefault = true;
                return;
            }

            languageInfo.IsDefault = true;
        }
    }
}
