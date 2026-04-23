using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 获取
    /// </summary>
    public class HomeMyCompletedAndUncompletedDto
    {
        /// <summary>
        /// 待办审批数量
        /// </summary>
        public int CompletedCount { get; set; }
        /// <summary>
        /// 已办审批数量
        /// </summary>
        public int UnCompletedCount { get; set; }
    }
}
