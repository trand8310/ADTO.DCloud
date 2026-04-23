using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EnumManager.EnumUtils
{
    /// <summary>
    /// 枚举基础标识属性
    /// </summary>
    public class EnumDescAttribute : Attribute
    {
        /// <summary>
        /// 枚举值对应中文意思
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 枚举值附加说明
        /// </summary>
        public string Descript { get; set; }
        public string Desc { get; private set; }

        public EnumDescAttribute(string Name, string Descript = "")
        {
            this.Name = Name;
            this.Descript = Descript;

        }
        public EnumDescAttribute(string desc)
        {
            this.Desc = desc;
        }

    }
}
