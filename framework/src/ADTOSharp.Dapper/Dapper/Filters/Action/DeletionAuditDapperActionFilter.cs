using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Extensions;
using ADTOSharp.Timing;
using System;

namespace ADTOSharp.Dapper.Filters.Action
{
    public class DeletionAuditDapperActionFilter : DapperActionFilterBase, IDapperActionFilter
    {
        public void ExecuteFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (entity is ISoftDelete)
            {
                var record = entity.As<ISoftDelete>();
                record.IsDeleted = true;
            }

            if (entity is IHasDeletionTime)
            {
                var record = entity.As<IHasDeletionTime>();
                if (record.DeletionTime == null)
                {
                    record.DeletionTime = Clock.Now;
                }
            }

            if (entity is IDeletionAudited)
            {
                Guid? userId = GetAuditUserId();
                var record = entity.As<IDeletionAudited>();

                if (record.DeleterUserId != null && record.DeleterUserId.HasValue && record.DeleterUserId != Guid.Empty)
                {
                    return;
                }

                if (userId == null)
                {
                    record.DeleterUserId = null;
                    return;
                }

                if (entity is IMayHaveTenant || entity is IMustHaveTenant)
                {
                    //Sets LastModifierUserId only if current user is in same tenant/host with the given entity
                    if (entity is IMayHaveTenant && entity.As<IMayHaveTenant>().TenantId == ADTOSharpSession.TenantId ||
                        entity is IMustHaveTenant && entity.As<IMustHaveTenant>().TenantId == ADTOSharpSession.TenantId)
                    {
                        record.DeleterUserId = userId;
                    }
                    else
                    {
                        record.DeleterUserId = null;
                    }
                }
                else
                {
                    record.DeleterUserId = userId;
                }
            }
        }
    }
}
