using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataSource.Dto
{
    public class GetDataTableQueryDto
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string sidx { get; set; }

        /// <summary>
        /// 查询条件集合
        /// </summary>
        public string paramsJson { get; set; }
    }
}
