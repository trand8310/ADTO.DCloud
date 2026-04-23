using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Abs.Dto
{

    public class GeFromObjectInput
    {
        ///// <summary>
        ///// 
        ///// </summary>
        //public string Data { get; set; }

        public List<GetFormObjectValue> Data { get; set; }
    }

    public class GetFormObjectValue
    {

        /// <summary>
        /// 字段
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string Value { get; set; }
    }

    public class BackFormsResult
    {
        public string Field { get; set; }

        public string Value { get; set; }
    }
}
