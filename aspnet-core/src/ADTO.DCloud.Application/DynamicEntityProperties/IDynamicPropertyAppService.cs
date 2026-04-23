using System;
using System.Threading.Tasks;
using ADTO.DCloud.DynamicEntityProperties.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.UI.Inputs;

namespace ADTO.DCloud.DynamicEntityProperties
{
    public interface IDynamicPropertyAppService
    {
        Task<DynamicPropertyDto> Get(Guid id);

        Task<ListResultDto<DynamicPropertyDto>> GetAll();

        Task Add(DynamicPropertyDto dto);

        Task Update(DynamicPropertyDto dto);

        Task Delete(EntityDto<Guid> input);

        IInputType FindAllowedInputType(string name);
    }
}
