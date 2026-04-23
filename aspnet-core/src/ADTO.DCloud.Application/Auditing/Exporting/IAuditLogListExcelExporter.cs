using System.Collections.Generic;
using ADTO.DCloud.Auditing.Dto;
using ADTO.DCloud.Dto;
 
namespace ADTO.DCloud.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);

        FileDto ExportToFile2(List<EntityChangeListDto> entityChangeListDtos);
    }
}
