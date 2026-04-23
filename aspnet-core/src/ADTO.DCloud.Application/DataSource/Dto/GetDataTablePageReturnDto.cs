using ADTO.DCloud.CodeTable;
using ADTO.DCloud.FormScheme.Dto;
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataSource.Dto
{
    /// <summary>
    /// 获取数据源数据分页查询返回对象
    /// </summary>
    public class PageReturnDataTableDto<T> : PagedResultDto<T>
    {
        public PageReturnDataTableDto(int totalCount, IReadOnlyList<T> items, List<DataSourceColumns> Columns) : base(totalCount, items)
        {
            cols = Columns;
        }
        public List<DataSourceColumns> cols { get; set; }

    }
}
