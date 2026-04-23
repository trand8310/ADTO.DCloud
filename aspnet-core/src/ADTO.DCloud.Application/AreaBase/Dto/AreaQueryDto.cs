using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.AreaBase.Dto
{
    /// <summary>
    /// 列表查询条件
    /// </summary>
    public class AreaQueryDto
    {
        /// <summary>
        /// 上级Id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 当前等级(1=大区；2=国家；3=省份；4=城市；5=区县)
        /// </summary>
        public int Level { get; set; }
    }
}
