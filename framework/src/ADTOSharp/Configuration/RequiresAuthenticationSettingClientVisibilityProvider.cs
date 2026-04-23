using System.Threading.Tasks;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Session;

namespace ADTOSharp.Configuration
{
    public class RequiresAuthenticationSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        public async Task<bool> CheckVisible(IScopedIocResolver scope)
        {
            return await Task.FromResult(
                scope.Resolve<IADTOSharpSession>().UserId.HasValue
            );
        }
    }
}