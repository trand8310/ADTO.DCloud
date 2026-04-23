using System;
using System.Threading.Tasks;
using ADTO.DCloud.DynamicEntityProperties.Dto;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyAppService
    {
        Task<DynamicEntityPropertyDto> Get(EntityDto<Guid> input);

        Task<ListResultDto<DynamicEntityPropertyDto>> GetAllPropertiesOfAnEntity(DynamicEntityPropertyGetAllInput input);


        Task<ListResultDto<DynamicEntityPropertyDto>> GetAll();

        Task Add(DynamicEntityPropertyDto dto);

        Task Update(DynamicEntityPropertyDto dto);

        Task Delete(EntityDto<Guid> input);
        
        Task<ListResultDto<GetAllEntitiesHasDynamicPropertyOutput>> GetAllEntitiesHasDynamicProperty();
    }
}
