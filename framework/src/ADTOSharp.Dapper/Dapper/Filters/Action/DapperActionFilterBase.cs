using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Reflection;
using ADTOSharp.Runtime.Session;

namespace ADTOSharp.Dapper.Filters.Action
{
    public abstract class DapperActionFilterBase
    {
        protected DapperActionFilterBase()
        {
            ADTOSharpSession = NullADTOSharpSession.Instance;
            GuidGenerator = SequentialGuidGenerator.Instance;
        }

        public IADTOSharpSession ADTOSharpSession { get; set; }

        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        public IGuidGenerator GuidGenerator { get; set; }

        protected virtual Guid? GetAuditUserId()
        {
            if (ADTOSharpSession.UserId.HasValue && CurrentUnitOfWorkProvider?.Current != null)
            {
                return ADTOSharpSession.UserId;
            }

            return null;
        }

        protected virtual void CheckAndSetId(object entityAsObj)
        {
            var entity = entityAsObj as IEntity<Guid>;
            if (entity != null && entity.Id == Guid.Empty)
            {
                Type entityType = entityAsObj.GetType();
                PropertyInfo idProperty = entityType.GetProperty("Id");
                var dbGeneratedAttr = ReflectionHelper.GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(idProperty);
                if (dbGeneratedAttr == null || dbGeneratedAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.None)
                {
                    entity.Id = GuidGenerator.Create();
                }
            }
        }

        protected virtual Guid? GetCurrentTenantIdOrNull()
        {
            if (CurrentUnitOfWorkProvider?.Current != null)
            {
                return CurrentUnitOfWorkProvider.Current.GetTenantId();
            }

            return ADTOSharpSession.TenantId;
        }
    }
}
