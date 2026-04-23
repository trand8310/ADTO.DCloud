using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CodeTable.Dto
{
    /// <summary>
    /// 获取数据表信息
    /// </summary>
    public class CodeTableDetailInfoDto
    {
        /// <summary>
        /// 表信息
        /// </summary>
        public CodeTableDto CodeTableInfo { get; set; }

        /// <summary>
        /// 字段栏位信息
        /// </summary>
        public List<CodeColumnsDto> CodeColumnsList { get; set; }
    }
}
