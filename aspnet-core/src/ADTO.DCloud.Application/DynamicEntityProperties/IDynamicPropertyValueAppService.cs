using System;
using System.Threading.Tasks;
using ADTO.DCloud.DynamicEntityProperties.Dto;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.DynamicEntityProperties
{
    public interface IDynamicPropertyValueAppService
    {
        Task<DynamicPropertyValueDto> Get(Guid id);

        Task<ListResultDto<DynamicPropertyValueDto>> GetAllValuesOfDynamicProperty(EntityDto<Guid> input);

        Task Add(DynamicPropertyValueDto dto);

        Task Update(DynamicPropertyValueDto dto);

        Task Delete(EntityDto<Guid> id);
    }
}
