using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes
{
    public interface IDataFilterService
    {
        Task<IQueryable<T>> CreateDataFilteredQuery<T>(IQueryable<T> query, string methodName);
    }
}
