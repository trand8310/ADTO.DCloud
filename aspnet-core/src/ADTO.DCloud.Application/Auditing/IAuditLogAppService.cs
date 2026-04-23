using ADTO.DCloud.Auditing.Dto;
using ADTO.DCloud.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 

namespace ADTO.DCloud.Auditing
{
    public interface IAuditLogAppService : IApplicationService
    {
        Task<PagedResultDto<AuditLogListDto>> GetAuditLogsPageListAsync(GetAuditLogsInput input);

        Task<FileDto> GetAuditLogsToExcelAsync(GetAuditLogsInput input);

        Task<PagedResultDto<EntityChangeListDto>> GetEntityChanges(GetEntityChangeInput input);

        Task<PagedResultDto<EntityChangeListDto>> GetEntityTypeChanges(GetEntityTypeChangeInput input);

        Task<FileDto> GetEntityChangesToExcel(GetEntityChangeInput input);

        Task<List<EntityPropertyChangeDto>> GetEntityPropertyChanges(Guid entityChangeId);

        List<NameValueDto> GetEntityHistoryObjectTypes();
    }
}