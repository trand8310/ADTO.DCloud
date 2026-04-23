using System.Globalization;

namespace ADTO.DCloud.Localization
{
    public static class CultureHelper
    {
        public static bool IsRtl => CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

        public static CultureInfo GetCultureInfoByChecking(string name)
        {
            try
            {
                return CultureInfo.GetCultureInfo(name);
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.CurrentCulture;
            }
        }
    }
}
