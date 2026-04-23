using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.AreaBase.Dto
{
    public class GetInfoAreaDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 当前等级(1=大区；2=国家；3=省份；4=城市；5=区县)
        /// </summary>
        public int Level { get; set; }
    }
}
