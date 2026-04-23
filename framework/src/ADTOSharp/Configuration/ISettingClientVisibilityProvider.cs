using System.Threading.Tasks;
using ADTOSharp.Dependency;

namespace ADTOSharp.Configuration
{
    public interface ISettingClientVisibilityProvider
    {
        Task<bool> CheckVisible(IScopedIocResolver scope);
    }
}