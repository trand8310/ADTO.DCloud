using ADTO.DCloud.Localization.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System;
using System.Threading.Tasks;


namespace ADTO.DCloud.Localization
{
    public interface ILanguageAppService : IApplicationService
    {
        Task<GetLanguagesOutput> GetLanguagesAsync();

        Task<GetLanguageForEditOutput> GetLanguageForEditAsync(NullableIdDto<Guid> input);

        Task CreateOrUpdateLanguageAsync(CreateOrUpdateLanguageInput input);

        Task DeleteLanguageAsync(EntityDto<Guid> input);

        Task SetDefaultLanguageAsync(SetDefaultLanguageInput input);

        Task<PagedResultDto<LanguageTextListDto>> GetLanguageTextsAsync(GetLanguageTextsInput input);

        Task UpdateLanguageTextAsync(UpdateLanguageTextInput input);
    }
}
