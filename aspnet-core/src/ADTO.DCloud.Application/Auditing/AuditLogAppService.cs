using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ADTO.DCloud.Auditing.Dto;
using ADTO.DCloud.Auditing.Exporting;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Dto;
using ADTO.DCloud.EntityHistory;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.EntityHistory;
using ADTOSharp.Extensions;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using EntityHistoryHelper = ADTO.DCloud.EntityHistory.EntityHistoryHelper;


namespace ADTO.DCloud.Auditing;

/// <summary>
/// “系统日志”页面使用的应用程序服务。
/// </summary>
[DisableAuditing]
[ADTOSharpAuthorize(PermissionNames.Pages_Administration_AuditLogs)]
public class AuditLogAppService : DCloudAppServiceBase, IAuditLogAppService
{
    #region Fields
    private readonly IRepository<AuditLog, long> _auditLogRepository;
    private readonly IRepository<EntityChange, Guid> _entityChangeRepository;
    private readonly IRepository<EntityChangeSet, Guid> _entityChangeSetRepository;
    private readonly IRepository<EntityPropertyChange, Guid> _entityPropertyChangeRepository;
    private readonly IRepository<User, Guid> _userRepository;
    private readonly IAuditLogListExcelExporter _auditLogListExcelExporter;
    private readonly INamespaceStripper _namespaceStripper;
    private readonly IADTOSharpStartupConfiguration _adtosharpStartupConfiguration;
    #endregion

    #region Ctor

    public AuditLogAppService(
        IRepository<AuditLog, long> auditLogRepository,
        IRepository<User, Guid> userRepository,
        IAuditLogListExcelExporter auditLogListExcelExporter,
        INamespaceStripper namespaceStripper,
        IRepository<EntityChange, Guid> entityChangeRepository,
        IRepository<EntityChangeSet, Guid> entityChangeSetRepository,
        IRepository<EntityPropertyChange, Guid> entityPropertyChangeRepository,
        IADTOSharpStartupConfiguration adtosharpStartupConfiguration)
    {
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
        _auditLogListExcelExporter = auditLogListExcelExporter;
        _namespaceStripper = namespaceStripper;
        _entityChangeRepository = entityChangeRepository;
        _entityChangeSetRepository = entityChangeSetRepository;
        _entityPropertyChangeRepository = entityPropertyChangeRepository;
        _adtosharpStartupConfiguration = adtosharpStartupConfiguration;
    }

    #endregion


    #region Utilities

    /// <summary>
    /// 实体转换
    /// </summary>
    /// <param name="results"></param>
    /// <returns></returns>
    private List<AuditLogListDto> ConvertToAuditLogListDtos(List<AuditLogAndUser> results)
    {
        return results.Select(
            result =>
            {
                var auditLogListDto = ObjectMapper.Map<AuditLogListDto>(result.AuditLog);
                auditLogListDto.UserName = result.User?.UserName;
                auditLogListDto.ServiceName = _namespaceStripper.StripNameSpace(auditLogListDto.ServiceName);
                return auditLogListDto;
            }).ToList();
    }

    /// <summary>
    /// 分页列表查询、导出条件组合（日期必传）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private IQueryable<AuditLogAndUser> CreateAuditLogAndUsersQuery(GetAuditLogsInput input)
    {
        var query = from auditLog in _auditLogRepository.GetAll()
                    join user in _userRepository.GetAll() on auditLog.UserId equals user.Id into userJoin
                    from joinedUser in userJoin.DefaultIfEmpty()
                    where auditLog.ExecutionTime >= input.StartDate && auditLog.ExecutionTime <= input.EndDate
                    select new AuditLogAndUser { AuditLog = auditLog, User = joinedUser };

        query = query
            .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
            .WhereIf(!input.ServiceName.IsNullOrWhiteSpace(), item => item.AuditLog.ServiceName.Contains(input.ServiceName))
            .WhereIf(!input.MethodName.IsNullOrWhiteSpace(), item => item.AuditLog.MethodName.Contains(input.MethodName))
            .WhereIf(!input.BrowserInfo.IsNullOrWhiteSpace(), item => item.AuditLog.BrowserInfo.Contains(input.BrowserInfo))
            .WhereIf(input.MinExecutionDuration.HasValue && input.MinExecutionDuration > 0, item => item.AuditLog.ExecutionDuration >= input.MinExecutionDuration.Value)
            .WhereIf(input.MaxExecutionDuration.HasValue && input.MaxExecutionDuration < int.MaxValue, item => item.AuditLog.ExecutionDuration <= input.MaxExecutionDuration.Value)
            .WhereIf(input.HasException == true, item => item.AuditLog.Exception != null && item.AuditLog.Exception != "")
            .WhereIf(input.HasException == false, item => item.AuditLog.Exception == null || item.AuditLog.Exception == "");
        return query;
    }


    private List<EntityChangeListDto> ConvertToEntityChangeListDtos(List<EntityChangeAndUser> results)
    {
        return results.Select(
            result =>
            {
                var entityChangeListDto = ObjectMapper.Map<EntityChangeListDto>(result.EntityChange);
                entityChangeListDto.UserName = result.User?.UserName;
                return entityChangeListDto;
            }).ToList();
    }

    private IQueryable<EntityChangeAndUser> CreateEntityChangesAndUsersQuery(GetEntityChangeInput input)
    {
        var query = from entityChangeSet in _entityChangeSetRepository.GetAll()
                    join entityChange in _entityChangeRepository.GetAll() on entityChangeSet.Id equals entityChange.EntityChangeSetId
                    join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                    where entityChange.ChangeTime >= input.StartDate && entityChange.ChangeTime <= input.EndDate
                    select new EntityChangeAndUser
                    {
                        EntityChange = entityChange,
                        User = user
                    };

        query = query
            .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
            .WhereIf(!input.EntityTypeFullName.IsNullOrWhiteSpace(), item => item.EntityChange.EntityTypeFullName.Contains(input.EntityTypeFullName));

        return query;
    }

    #endregion

    #region Methods

    #region audit logs

    /// <summary>
    /// 日志分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<AuditLogListDto>> GetAuditLogsPageListAsync(GetAuditLogsInput input)
    {
        var query = CreateAuditLogAndUsersQuery(input);

        var resultCount = await query.CountAsync();
        var results = await query
            .OrderBy(input.Sorting)
            .PageBy(input)
            .ToListAsync();

        var auditLogListDtos = ConvertToAuditLogListDtos(results);

        return new PagedResultDto<AuditLogListDto>(resultCount, auditLogListDtos);
    }
    /// <summary>
    /// 导出日志到EXCEL
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<FileDto> GetAuditLogsToExcelAsync(GetAuditLogsInput input)
    {
        var auditLogs = await CreateAuditLogAndUsersQuery(input)
            .AsNoTracking()
            .OrderByDescending(al => al.AuditLog.ExecutionTime)
            .ToListAsync();

        var auditLogListDtos = ConvertToAuditLogListDtos(auditLogs);

        return _auditLogListExcelExporter.ExportToFile(auditLogListDtos);
    }

    #endregion

    #region entity changes 

    public List<NameValueDto> GetEntityHistoryObjectTypes()
    {
        var entityHistoryObjectTypes = new List<NameValueDto>();
        var enabledEntities = (_adtosharpStartupConfiguration.GetCustomConfig()
            .FirstOrDefault(x => x.Key == EntityHistoryHelper.EntityHistoryConfigurationName)
            .Value as EntityHistoryUiSetting)?.EnabledEntities ?? new List<string>();

        if (ADTOSharpSession.TenantId == null)
        {
            enabledEntities = EntityHistoryHelper.HostSideTrackedTypes.Select(t => t.FullName).Intersect(enabledEntities).ToList();
        }
        else
        {
            enabledEntities = EntityHistoryHelper.TenantSideTrackedTypes.Select(t => t.FullName).Intersect(enabledEntities).ToList();
        }

        foreach (var enabledEntity in enabledEntities)
        {
            entityHistoryObjectTypes.Add(new NameValueDto(L(enabledEntity), enabledEntity));
        }

        return entityHistoryObjectTypes;
    }

    public async Task<PagedResultDto<EntityChangeListDto>> GetEntityChanges(GetEntityChangeInput input)
    {
        var query = CreateEntityChangesAndUsersQuery(input);

        var resultCount = await query.CountAsync();
        var results = await query
            .OrderBy(input.Sorting)
            .PageBy(input)
            .ToListAsync();

        var entityChangeListDtos = ConvertToEntityChangeListDtos(results);

        return new PagedResultDto<EntityChangeListDto>(resultCount, entityChangeListDtos);
    }

    public async Task<PagedResultDto<EntityChangeListDto>> GetEntityTypeChanges(GetEntityTypeChangeInput input)
    {

        var entityId = "\"" + input.EntityId + "\"";

        var query = from entityChangeSet in _entityChangeSetRepository.GetAll()
                    join entityChange in _entityChangeRepository.GetAll() on entityChangeSet.Id equals entityChange.EntityChangeSetId
                    join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                    where entityChange.EntityTypeFullName == input.EntityTypeFullName &&
                          (entityChange.EntityId == input.EntityId || entityChange.EntityId == entityId)
                    select new EntityChangeAndUser
                    {
                        EntityChange = entityChange,
                        User = user
                    };

        var resultCount = await query.CountAsync();
        var results = await query
            .OrderBy(input.Sorting)
            .PageBy(input)
            .ToListAsync();

        var entityChangeListDtos = ConvertToEntityChangeListDtos(results);

        return new PagedResultDto<EntityChangeListDto>(resultCount, entityChangeListDtos);
    }

    public async Task<FileDto> GetEntityChangesToExcel(GetEntityChangeInput input)
    {
        var entityChanges = await CreateEntityChangesAndUsersQuery(input)
            .AsNoTracking()
            .OrderByDescending(ec => ec.EntityChange.EntityChangeSetId)
            .ThenByDescending(ec => ec.EntityChange.ChangeTime)
            .ToListAsync();

        var entityChangeListDtos = ConvertToEntityChangeListDtos(entityChanges);

        return _auditLogListExcelExporter.ExportToFile2(entityChangeListDtos);
    }

    public async Task<List<EntityPropertyChangeDto>> GetEntityPropertyChanges(Guid entityChangeId)
    {
        var entityPropertyChanges = await _entityPropertyChangeRepository.GetAllListAsync(epc => epc.EntityChangeId == entityChangeId);

        return ObjectMapper.Map<List<EntityPropertyChangeDto>>(entityPropertyChanges);
    }


    #endregion

    #endregion
}

