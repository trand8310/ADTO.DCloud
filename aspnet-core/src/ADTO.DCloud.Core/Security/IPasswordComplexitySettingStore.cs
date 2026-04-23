using System.Threading.Tasks;

namespace ADTO.DCloud.Security
{
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}
