using System.Collections.Generic;

namespace ADTOSharp.Web.Models.ADTOSharpUserConfiguration
{
    public class ADTOSharpUserConfigurationDto
    {
        public ADTOSharpMultiTenancyConfigDto MultiTenancy { get; set; }

        public ADTOSharpUserSessionConfigDto Session { get; set; }

        public ADTOSharpUserLocalizationConfigDto Localization { get; set; }

        public ADTOSharpUserFeatureConfigDto Features { get; set; }

        public ADTOSharpUserAuthConfigDto Auth { get; set; }

        public ADTOSharpUserNavConfigDto Nav { get; set; }

        public ADTOSharpUserSettingConfigDto Setting { get; set; }

        public ADTOSharpUserClockConfigDto Clock { get; set; }

        public ADTOSharpUserTimingConfigDto Timing { get; set; }

        public ADTOSharpUserSecurityConfigDto Security { get; set; }

        public Dictionary<string, object> Custom { get; set; }
    }
}