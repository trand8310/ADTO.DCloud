using System.Collections.Generic;
using ADTOSharp.Dapper_Extensions.Sql;

namespace ADTOSharp.Dapper_Extensions.Predicate
{
    public interface IPredicate
    {
        string GetSql(ISqlGenerator sqlGenerator, IDictionary<string, object> parameters, bool isDml = false);
    }
}
