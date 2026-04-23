using ADTOSharp.Auditing;
using System.Collections.Generic;
 
namespace ADTO.DCloud.Auditing
{
    public interface IExpiredAndDeletedAuditLogBackupService
    {
        bool CanBackup();
        
        void Backup(List<AuditLog> auditLogs);
    }
}