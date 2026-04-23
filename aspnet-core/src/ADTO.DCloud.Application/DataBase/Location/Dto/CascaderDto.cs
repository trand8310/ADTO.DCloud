using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataBase.Location.Dto
{
    public class CascaderDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Value { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 是否最后一级
        /// </summary>
        public bool Leaf { get; set; }
    }
}
