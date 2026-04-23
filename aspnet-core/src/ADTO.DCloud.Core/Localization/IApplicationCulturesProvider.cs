using System.Globalization;

namespace ADTO.DCloud.Localization
{
    public interface IApplicationCulturesProvider
    {
        CultureInfo[] GetAllCultures();
    }
}