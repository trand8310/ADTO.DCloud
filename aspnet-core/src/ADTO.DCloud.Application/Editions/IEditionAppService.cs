using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.Editions.Dto;
using System;

namespace ADTO.DCloud.Editions
{
    public interface IEditionAppService : IApplicationService
    {
        Task<ListResultDto<EditionListDto>> GetEditions();

        Task<GetEditionEditOutput> GetEditionForEdit(NullableIdDto<Guid> input);

        Task CreateEdition(CreateEditionDto input);

        Task UpdateEdition(UpdateEditionDto input);

        Task DeleteEdition(EntityDto<Guid> input);

        Task MoveTenantsToAnotherEdition(MoveTenantsToAnotherEditionDto input);

        Task<List<SubscribableEditionComboboxItemDto>> GetEditionComboboxItems(int? selectedEditionId = null, bool addAllItem = false, bool onlyFree = false);

        Task<int> GetTenantCount(Guid editionId);
    }
}