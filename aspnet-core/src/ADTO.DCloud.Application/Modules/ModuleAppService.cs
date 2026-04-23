using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.Dapper;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Modules.Dto;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Configuration;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.Modules;

/// <summary>
/// 模块管理服务
/// </summary>
public class ModuleAppService : DCloudAppServiceBase, IModuleAppService
{
    private readonly IRepository<Module, Guid> _moduleRepository;
    private readonly IRepository<ModuleItem, Guid> _moduleItemRepository;
    //private readonly ICacheManager _cacheManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly ISettingManager _settingManager;
    private readonly IDapperSqlExecutor _sqlExecutor;

    /// <summary>
    /// ModuleAppService
    /// </summary>
    /// <param name="moduleRepository"></param>
    /// <param name="moduleItemRepository"></param>
    /// <param name="cacheManager"></param>
    public ModuleAppService(IRepository<Module, Guid> moduleRepository
        , IRepository<ModuleItem, Guid> moduleItemRepository
        , IUnitOfWorkManager unitOfWorkManager
        , ISettingManager settingManager,
        IDapperSqlExecutor sqlExecutor
        //, ICacheManager cacheManager
        )
    {
        _moduleRepository = moduleRepository;
        _moduleItemRepository = moduleItemRepository;
        //_cacheManager = cacheManager;
        _unitOfWorkManager = unitOfWorkManager;
        _settingManager = settingManager;
        _sqlExecutor = sqlExecutor;



    }
    public async Task<ListResultDto<UserDto>> GetAllModulesv2()
    {
        var list = await _sqlExecutor.QueryAsync<UserDto>("SELECT * FROM Users");
        return new ListResultDto<UserDto>
        {
            Items = list.ToList()
        };

    }
    #region Module
    /// <summary>
    /// 获取功能列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<ModuleDto>> GetModulesAsync(GetModulesInput input)
    {



        var query = _moduleRepository.GetAllIncluding(x => x.ModuleItems, p => p.Parent);
        query = query.WhereIf(input.ParentId.HasValue, w => w.Parent != null && w.Parent.Id == input.ParentId.Value);
        query = query.WhereIf(!input.ParentId.HasValue, w => w.Parent == null);
        var totalCount = query.Count();
        var modules = await query.PageBy(input).ToListAsync();

        var list = modules.Select(item =>
        {
            var moduleDto = ObjectMapper.Map<ModuleDto>(item);
            if (item.Parent != null)
                moduleDto.ParentId = item.Parent.Id;

            moduleDto.ModuleItems = ObjectMapper.Map<List<ModuleItemDto>>(item.ModuleItems);
            return moduleDto;
        }).ToList();
        return new PagedResultDto<ModuleDto>(totalCount, list);
    }

    /// <summary>
    /// 获取模块(依Id)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ModuleEditDto> GetModuleForEditAsync(EntityDto<Guid> input)
    {
        var module = await _moduleRepository.GetAsync(input.Id);
        var moduleDto = ObjectMapper.Map<ModuleEditDto>(module);
        return moduleDto;
    }



    /// <summary>
    /// 获取模块(依Id)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ModuleDto> GetModuleAsync(EntityDto<Guid> input)
    {
        var module = await _moduleRepository.GetAsync(input.Id);
        var moduleDto = ObjectMapper.Map<ModuleDto>(module);
        return moduleDto;
    }
    /// <summary>
    /// 新增模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ModuleDto> CreateModuleAsync(CreateModuleInput input)
    {
        var module = ObjectMapper.Map<Module>(input);
        if (input.ParentId.HasValue)
        {
            var parent = await _moduleRepository.GetAsync(input.ParentId.Value);
            if (parent != null)
                module.Parent = parent;
        }

        await _moduleRepository.InsertAsync(module);
        CurrentUnitOfWork.SaveChanges();
        return ObjectMapper.Map<ModuleDto>(module);
    }
    /// <summary>
    /// 修改模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ModuleDto> UpdateModuleAsync(UpdateModuleInput input)
    {
        var module = await _moduleRepository.GetAsync(input.Id);
        ObjectMapper.Map(input, module);
        if (input.ParentId.HasValue)
        {
            var parent = await _moduleRepository.GetAsync(input.ParentId.Value);
            if (parent != null)
                module.Parent = parent;
        }
        await _moduleRepository.UpdateAsync(module);
        return await GetModuleAsync(input);
    }

    /// <summary>
    /// 删除模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeleteModuleAsync(EntityDto<Guid> input)
    {
        await _moduleItemRepository.DeleteAsync(x => x.ModuleId == input.Id);
        await _moduleRepository.DeleteAsync(input.Id);
    }

    /// <summary>
    /// 获取菜单树状视图
    /// </summary>
    /// <returns></returns>
    public async Task<List<ModuleTreeListDto>> GetModulesTreeViewAsync()
    {
        var list = await _moduleRepository.GetAll().ToListAsync();
        var modules = (list.OrderBy(d=>d.DisplayOrder).Select(item =>
        {
            var moduleDto = ObjectMapper.Map<ModuleDto>(item);
            return moduleDto;
        }).ToList());

        var data = GenerateModuleTreeView(modules, null);
        return data;
    }

    private List<ModuleTreeListDto> GenerateModuleTreeView(List<ModuleDto> list, Guid? parentId = null)
    {
        var query = list.Where(w => w.ParentId == parentId);
        return query.Select(item =>
        {
            var model = ObjectMapper.Map<ModuleTreeListDto>(item);
            model.ParentId = item.ParentId.HasValue ? item.ParentId : null;
            model.Children = GenerateModuleTreeView(list, item.Id);
            return model;
        }).ToList();
    }


    #endregion

    #region ModuleItems
    /// <summary>
    /// 获取模块元素(依ModuleId)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ListResultDto<ModuleItemDto>> GetModuleItemsAsync(EntityDto<Guid> input)
    {
        var list = await _moduleItemRepository.GetAllIncluding(x => x.Module).Where(w => w.ModuleId == input.Id).ToListAsync();
        return new ListResultDto<ModuleItemDto>(
            list.Select(mi =>
            {
                var moduleItemDto = ObjectMapper.Map<ModuleItemDto>(mi);
                if (!mi.Module.Permission.IsNullOrWhiteSpace())
                    moduleItemDto.FullPermission = moduleItemDto.Permission.Contains(".") ? moduleItemDto.Permission : $"{mi.Module.Permission}.{moduleItemDto.Permission}";
                else
                    moduleItemDto.FullPermission = moduleItemDto.Permission;


                return moduleItemDto;
            }).ToList());
    }

    /// <summary>
    /// 获取模块元素(依Id)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ModuleItemDto> GetModuleItemAsync(EntityDto<Guid> input)
    {
        var moduleItem = await _moduleItemRepository.GetAsync(input.Id);
        var moduleItemDto = ObjectMapper.Map<ModuleItemDto>(moduleItem);
        return moduleItemDto;
    }

    /// <summary>
    /// 新增模块元素
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ModuleItemDto> CreateModuleItemAsync(CreateModuleItemInput input)
    {
        var module = ObjectMapper.Map<ModuleItem>(input);
        module.TenantId = ADTOSharpSession.TenantId;
        await _moduleItemRepository.InsertAsync(module);
        CurrentUnitOfWork.SaveChanges();
        return ObjectMapper.Map<ModuleItemDto>(module);
    }

    /// <summary>
    /// 修改模块元素
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ModuleItemDto> UpdateModuleItemAsync(UpdateModuleItemInput input)
    {
        var moduleItem = await _moduleItemRepository.GetAsync(input.Id);
        ObjectMapper.Map(input, moduleItem);
        await _moduleItemRepository.UpdateAsync(moduleItem);
        return await GetModuleItemAsync(input);
    }

    /// <summary>
    /// 删除模块元素
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeleteModuleItemAsync(EntityDto<Guid> input)
    {
        await _moduleItemRepository.DeleteAsync(x => x.Id == input.Id);
    }
    #endregion

    #region 手机端相关

    /// <summary>
    /// 获取控制面板菜单
    /// </summary>
    /// <returns></returns>
    [ADTOSharpAuthorize]
    public string GetWorkPanelMenu()
    {
        var userId = ADTOSharpSession.ToUserIdentifier();
        if (userId == null)
        {
            throw new UserFriendlyException("保存失败,当前用户没登录！");
        }
        var menuJson = _settingManager.GetSettingValueForUser(AppSettings.UserManagement.MiniProgram.WorkPanelMenu, ADTOSharpSession.ToUserIdentifier());
        return menuJson;
    }

    /// <summary>
    /// 设置控制面板菜单
    /// </summary>
    [ADTOSharpAuthorize]
    public void SetWorkPanelMenu(SetWorkPanelMenuDto input)
    {
        var userId = ADTOSharpSession.ToUserIdentifier();
        if (userId == null)
        {
            throw new UserFriendlyException("保存失败,当前用户没登录！");
        }
        _settingManager.ChangeSettingForUser(userId, AppSettings.UserManagement.MiniProgram.WorkPanelMenu, input.MenuJson);
    }

    /// <summary>
    /// 获取所有APP端的菜单
    /// </summary>
    /// <returns></returns>
    [ADTOSharpAuthorize]
    public async Task<List<ModelAppDto>> GetMenuListApp()
    {
        var list = await _moduleRepository.GetAllReadonly().Where(p => p.IsActive && p.IsShowApp == true).OrderBy(p => p.DisplayOrder).ToListAsync();
        var menus = (list.Select(item =>
        {
            var moduleDto = ObjectMapper.Map<ModelAppDto>(item);
            return moduleDto;
        }).ToList());

        return menus;
    }


    #endregion
}
