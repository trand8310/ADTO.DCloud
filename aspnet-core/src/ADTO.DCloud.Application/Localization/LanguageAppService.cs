using ADTO.DCloud.Localization.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using ADTOSharp.Localization;
using ADTOSharp.MultiTenancy;
using ADTOSharp.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ADTO.DCloud.Localization;

/// <summary>
/// “语言管理”页面使用的应用程序服务。
/// </summary>
public class LanguageAppService : DCloudAppServiceBase, ILanguageAppService
{
    #region Fields
    private readonly IApplicationLanguageManager _applicationLanguageManager;
    private readonly IApplicationLanguageTextManager _applicationLanguageTextManager;
    private readonly IRepository<ApplicationLanguage, Guid> _languageRepository;
    private readonly IApplicationCulturesProvider _applicationCulturesProvider;
    private readonly IRepository<ApplicationLanguageText, Guid> _customLocalizationRepository;

    private readonly IUnitOfWorkManager _unitOfWorkManager;
    #endregion

    #region Ctor
    public LanguageAppService(
        IApplicationLanguageManager applicationLanguageManager,
        IApplicationLanguageTextManager applicationLanguageTextManager,
        IRepository<ApplicationLanguage, Guid> languageRepository,
        IApplicationCulturesProvider applicationCulturesProvider,
        IRepository<ApplicationLanguageText, Guid> customLocalizationRepository,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _applicationLanguageManager = applicationLanguageManager;
        _languageRepository = languageRepository;
        _applicationLanguageTextManager = applicationLanguageTextManager;
        _applicationCulturesProvider = applicationCulturesProvider;
        _customLocalizationRepository = customLocalizationRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }
    #endregion

    #region Utilities


    protected virtual async Task CreateLanguageAsync(CreateOrUpdateLanguageInput input)
    {
        if (ADTOSharpSession.MultiTenancySide != MultiTenancySides.Host)
        {
            throw new UserFriendlyException(L("TenantsCannotCreateLanguage"));
        }

        var culture = CultureHelper.GetCultureInfoByChecking(input.Language.Name);

        await CheckLanguageIfAlreadyExists(culture.Name);

        await _applicationLanguageManager.AddAsync(
            new ApplicationLanguage(
                ADTOSharpSession.TenantId,
                culture.Name,
                culture.DisplayName,
                input.Language.Icon
            )
            {
                IsDisabled = !input.Language.IsEnabled
            }
        );
    }


    protected virtual async Task UpdateLanguageAsync(CreateOrUpdateLanguageInput input)
    {
        Debug.Assert(input.Language.Id != null, "input.Language.Id != null");

        var culture = CultureHelper.GetCultureInfoByChecking(input.Language.Name);

        await CheckLanguageIfAlreadyExists(culture.Name, input.Language.Id.Value);

        var language = await _languageRepository.GetAsync(input.Language.Id.Value);

        language.Name = culture.Name;
        language.DisplayName = culture.DisplayName;
        language.Icon = input.Language.Icon;
        language.IsDisabled = !input.Language.IsEnabled;

        await _applicationLanguageManager.UpdateAsync(ADTOSharpSession.TenantId, language);
    }

    private async Task CheckLanguageIfAlreadyExists(string languageName, Guid? expectedId = null)
    {
        var existingLanguage = (await _applicationLanguageManager.GetLanguagesAsync(ADTOSharpSession.TenantId))
            .FirstOrDefault(l => l.Name == languageName);

        if (existingLanguage == null)
        {
            return;
        }

        if (expectedId != null && existingLanguage.Id == expectedId.Value)
        {
            return;
        }

        throw new UserFriendlyException(L("ThisLanguageAlreadyExists"));
    }

    private string GetValueOrNull(List<string> items, int index)
    {
        return items.Count > index ? items[index] : null;
    }

    #endregion

    #region Methods

    /// <summary>
    /// 语言列表
    /// </summary>
    /// <returns></returns>
    public async Task<GetLanguagesOutput> GetLanguagesAsync()
    {
        var languages =
            (await _applicationLanguageManager.GetLanguagesAsync(ADTOSharpSession.TenantId)).OrderBy(l => l.DisplayName);
        var defaultLanguage = await _applicationLanguageManager.GetDefaultLanguageOrNullAsync(ADTOSharpSession.TenantId);
        return new GetLanguagesOutput(
            ObjectMapper.Map<List<ApplicationLanguageListDto>>(languages),
            defaultLanguage?.Name
        );
    }

    /// <summary>
    /// 获取一个编辑时使用的语言Dto
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<GetLanguageForEditOutput> GetLanguageForEditAsync(NullableIdDto<Guid> input)
    {
        ApplicationLanguage language = null;
        if (input.Id.HasValue)
        {
            language = await _languageRepository.GetAsync(input.Id.Value);
        }

        var output = new GetLanguageForEditOutput();

        //Language
        output.Language = language != null
            ? ObjectMapper.Map<ApplicationLanguageEditDto>(language)
            : new ApplicationLanguageEditDto();

        //Language names
        output.LanguageNames = _applicationCulturesProvider
            .GetAllCultures()
            .Select(c => new ComboboxItemDto(c.Name, c.EnglishName + " (" + c.Name + ")")
            { IsSelected = output.Language.Name == c.Name })
            .ToList();

        //Flags
        output.Flags = FamFamFamFlagsHelper
            .FlagClassNames
            .OrderBy(f => f)
            .Select(f => new ComboboxItemDto(f, FamFamFamFlagsHelper.GetCountryCode(f))
            { IsSelected = output.Language.Icon == f })
            .ToList();

        return output;
    }

    /// <summary>
    /// 创建/更新语言信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task CreateOrUpdateLanguageAsync(CreateOrUpdateLanguageInput input)
    {
        if (input.Language.Id.HasValue)
        {
            await UpdateLanguageAsync(input);
        }
        else
        {
            await CreateLanguageAsync(input);
        }
    }
    /// <summary>
    /// 删除语言
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeleteLanguageAsync(EntityDto<Guid> input)
    {
        var language = await _languageRepository.GetAsync(input.Id);
        await _applicationLanguageManager.RemoveAsync(ADTOSharpSession.TenantId, language.Name);
    }
    /// <summary>
    /// 设置默认语言
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task SetDefaultLanguageAsync(SetDefaultLanguageInput input)
    {
        await _applicationLanguageManager.SetDefaultLanguageAsync(
            ADTOSharpSession.TenantId,
            CultureHelper.GetCultureInfoByChecking(input.Name).Name
        );
    }

    protected virtual Dictionary<string, string> GetAllValuesFromDatabase(string sourceName,string languageName)
    {
        return _unitOfWorkManager.WithUnitOfWork(() =>
        {
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                return _customLocalizationRepository
                    .GetAll()
                    .Where(l => l.Source == sourceName &&
                                l.LanguageName == languageName)
                    .OrderBy(l => l.Id)
                    .ToDictionary(l => l.Key, l => l.Value);
            }
        });



        //return _unitOfWorkManager.WithUnitOfWork(() =>
        //{
        //    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        //    {
        //        return _customLocalizationRepository
        //            .GetAllList(l => l.Source == _sourceName && l.LanguageName == CultureInfo.Name)
        //            .ToDictionary(l => l.Key, l => l.Value);
        //    }
        //});
    }


    /// <summary>
    /// 获取语言的名称值对列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<PagedResultDto<LanguageTextListDto>> GetLanguageTextsAsync(GetLanguageTextsInput input)
    {
        if (input.BaseLanguageName.IsNullOrEmpty())
        {
            var defaultLanguage =
                await _applicationLanguageManager.GetDefaultLanguageOrNullAsync(ADTOSharpSession.TenantId);
            if (defaultLanguage == null)
            {
                defaultLanguage = (await _applicationLanguageManager.GetLanguagesAsync(ADTOSharpSession.TenantId))
                    .FirstOrDefault();
                if (defaultLanguage == null)
                {
                    throw new Exception("No language found in the application!");
                }
            }

            input.BaseLanguageName = defaultLanguage.Name;
        }
        var dict = GetAllValuesFromDatabase("DCloud", "zh-Hans");

        var source = LocalizationManager.GetSource(input.SourceName);
        var baseCulture = CultureInfo.GetCultureInfo(input.BaseLanguageName);
        var targetCulture = CultureInfo.GetCultureInfo(input.TargetLanguageName);

        var allStrings = source.GetAllStrings();
        var baseValues = _applicationLanguageTextManager.GetStringsOrNull(
            ADTOSharpSession.TenantId,
            source.Name,
            baseCulture,
            allStrings.Select(x => x.Name).ToList()
        );

        var targetValues = _applicationLanguageTextManager.GetStringsOrNull(
            ADTOSharpSession.TenantId,
            source.Name,
            targetCulture,
            allStrings.Select(x => x.Name).ToList()
        );

        var languageTexts = allStrings.Select((t, i) => new LanguageTextListDto
        {
            Key = t.Name,
            BaseValue = GetValueOrNull(baseValues, i),
            TargetValue = GetValueOrNull(targetValues, i) ?? GetValueOrNull(baseValues, i)
        }).AsQueryable();

        //Filters
        if (input.TargetValueFilter == "EMPTY")
        {
            languageTexts = languageTexts.Where(s => s.TargetValue.IsNullOrEmpty());
        }

        if (!input.FilterText.IsNullOrEmpty())
        {
            languageTexts = languageTexts.Where(
                l => (l.Key != null &&
                      l.Key.IndexOf(input.FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0) ||
                     (l.BaseValue != null &&
                      l.BaseValue.IndexOf(input.FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0) ||
                     (l.TargetValue != null &&
                      l.TargetValue.IndexOf(input.FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0)
            );
        }

        var totalCount = languageTexts.Count();

        //Ordering
        if (!input.Sorting.IsNullOrEmpty())
        {
            languageTexts = languageTexts.OrderBy(input.Sorting);
        }


        //Paging
        if (input.PageNumber > 0)
        {
            var skipCount = (input.PageNumber - 1) * input.PageSize;
            languageTexts = languageTexts.Skip(skipCount);
        }

        if (input.PageSize > 0)
        {
            languageTexts = languageTexts.Take(input.PageSize);
        }

        return new PagedResultDto<LanguageTextListDto>(
            totalCount,
            languageTexts.ToList()
        );
    }
    /// <summary>
    /// 更新某个语言的某个KEY值文本
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateLanguageTextAsync(UpdateLanguageTextInput input)
    {
        var culture = CultureHelper.GetCultureInfoByChecking(input.LanguageName);
        var source = LocalizationManager.GetSource(input.SourceName);
        await _applicationLanguageTextManager.UpdateStringAsync(
            ADTOSharpSession.TenantId,
            source.Name,
            culture,
            input.Key,
            input.Value
        );
    }

    /// <summary>
    /// 删除某个语言的某个KEY值文本
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    public async Task DeleteLanguageTextAsync(DeleteLanguageTextInput input)
    {
        var culture = CultureHelper.GetCultureInfoByChecking(input.LanguageName);
        var source = LocalizationManager.GetSource(input.SourceName);
        await _applicationLanguageTextManager.DeleteStringAsync(
            ADTOSharpSession.TenantId,
            source.Name,
            culture,
            input.Key
        );
    }
    #endregion

}

