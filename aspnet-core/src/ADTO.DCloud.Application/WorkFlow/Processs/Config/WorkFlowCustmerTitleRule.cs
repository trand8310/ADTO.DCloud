using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 描 述：工作流自定义标题
    /// </summary>
    public class WorkFlowCustmerTitleRule
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
    }
}
