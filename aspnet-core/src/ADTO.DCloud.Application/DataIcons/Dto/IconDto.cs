using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataIcons.Dto
{
    /// <summary>
    /// 系统图标
    /// </summary>
    /// 
    public class IconDto
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Ver { get; set; }
        /// <summary>
        /// 图标集合
        /// </summary>
        public IEnumerable<DataIconsDto> List { get; set; }
    }
}
