using System;
using System.Threading.Tasks;
using ADTO.DCloud.DynamicEntityProperties.Dto;
using ADTO.DCloud.DynamicEntityPropertyValues.Dto;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyValueAppService
    {
        Task<DynamicEntityPropertyValueDto> Get(Guid id);

        Task<ListResultDto<DynamicEntityPropertyValueDto>> GetAll(GetAllInput input);

        Task Add(DynamicEntityPropertyValueDto input);

        Task Update(DynamicEntityPropertyValueDto input);

        Task Delete(Guid id);

        Task<GetAllDynamicEntityPropertyValuesOutput> GetAllDynamicEntityPropertyValues(GetAllDynamicEntityPropertyValuesInput input);
    }
}
