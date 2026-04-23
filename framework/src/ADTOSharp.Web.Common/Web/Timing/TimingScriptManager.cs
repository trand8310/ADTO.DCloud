using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Extensions;
using ADTOSharp.Timing;
using ADTOSharp.Timing.Timezone;

namespace ADTOSharp.Web.Timing
{
    /// <summary>
    /// This class is used to build timing script.
    /// </summary>
    public class TimingScriptManager : ITimingScriptManager, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        public TimingScriptManager(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public async Task<string> GetScriptAsync()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");

            script.AppendLine("    adto.clock.provider = adto.timing." + Clock.Provider.GetType().Name.ToCamelCase() + " || adto.timing.localClockProvider;");
            script.AppendLine("    adto.clock.provider.supportsMultipleTimezone = " + Clock.SupportsMultipleTimezone.ToString().ToLowerInvariant() + ";");

            if (Clock.SupportsMultipleTimezone)
            {
                script.AppendLine("    adto.timing.timeZoneInfo = " + await GetUsersTimezoneScriptsAsync());
            }

            script.Append("})();");

            return script.ToString();
        }

        private async Task<string> GetUsersTimezoneScriptsAsync()
        {
            var timezoneId = await _settingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);
            var timezone = TimezoneHelper.FindTimeZoneInfo(timezoneId);

            return " {" +
                   "        windows: {" +
                   "            timeZoneId: '" + timezoneId + "'," +
                   "            baseUtcOffsetInMilliseconds: '" + timezone.BaseUtcOffset.TotalMilliseconds + "'," +
                   "            currentUtcOffsetInMilliseconds: '" + timezone.GetUtcOffset(Clock.Now).TotalMilliseconds + "'," +
                   "            isDaylightSavingTimeNow: '" + timezone.IsDaylightSavingTime(Clock.Now) + "'" +
                   "        }," +
                   "        iana: {" +
                   "            timeZoneId:'" + TimezoneHelper.WindowsToIana(timezoneId) + "'" +
                   "        }," +
                   "    }";
        }
    }
}