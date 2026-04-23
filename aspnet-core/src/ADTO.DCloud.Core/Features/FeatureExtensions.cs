

using ADTOSharp.Application.Features;
using ADTOSharp.Localization;

namespace ADTO.DCloud.Features
{
    public static class FeatureExtensions
    {
        public static string GetValueText(this Feature feature, string value, ILocalizationContext localizationContext)
        {
            var featureMetadata = feature[FeatureMetadata.CustomFeatureKey] as FeatureMetadata;
            if (featureMetadata?.ValueTextNormalizer == null)
            {
                return value;
            }

            return featureMetadata.ValueTextNormalizer(value).Localize(localizationContext);
        }
    }
}
