using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataArea.Dto
{
    /// <summary>
    /// 区域列表查询条件
    /// </summary>
    public class DataAreaListQueryDto
    {
        /// <summary>
        /// 父级Id  空值或0=顶层（省）
        /// </summary>
        public string ParentId { get; set; }
    }

}
