using System;
using System.Threading.Tasks;

namespace ADTOSharp.Application.Features
{
    public interface IADTOSharpZeroFeatureValueStore : IFeatureValueStore
    {
        Task<string> GetValueOrNullAsync(Guid tenantId, string featureName);
        string GetValueOrNull(Guid tenantId, string featureName);
        Task<string> GetEditionValueOrNullAsync(Guid editionId, string featureName);
        string GetEditionValueOrNull(Guid editionId, string featureName);
        Task SetEditionFeatureValueAsync(Guid editionId, string featureName, string value);
        void SetEditionFeatureValue(Guid editionId, string featureName, string value);
    }
}