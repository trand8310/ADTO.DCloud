using System;
using System.Collections.Generic;
using ADTOSharp.Domain.Entities;

namespace ADTOSharp.EntityFramework
{
    public interface IDbContextEntityFinder
    {
        IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType);
    }
}