using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataSource.Dto
{
    public  class PagedCodeListResultDto
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public DataTable rows { get; set; }

        /// <summary>
        /// 列的信息
        /// </summary>
        public List<DataSourceColumns> cols { get; set; }
    }
}
