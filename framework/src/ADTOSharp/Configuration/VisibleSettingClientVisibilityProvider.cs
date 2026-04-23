using System.Threading.Tasks;
using ADTOSharp.Dependency;

namespace ADTOSharp.Configuration
{
    public class VisibleSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        public async Task<bool> CheckVisible(IScopedIocResolver scope)
        {
            return await Task.FromResult(true);
        }
    }
}