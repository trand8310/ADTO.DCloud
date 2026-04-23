using ADTO.DCloud.WorkFlow.Processs.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 工作流引擎配置
    /// </summary>
    public class WFEngineConfig
    {
        /// <summary>
        /// 运行参数
        /// </summary>
        public WFParams Params { get; set; }

        /// <summary>
        /// 获取等待任务
        /// </summary>
        public GetAwaitTaskListMethod GetAwaitTaskList { get; set; }
        /// <summary>
        /// 获取审批人
        /// </summary>
        public GetUserListMethod GetUserList { get; set; }
        /// <summary>
        /// 获取单元上一次的审批人
        /// </summary>
        public GetRreTaskUserListMethod GetPrevTaskUserList { get; set; }
        /// <summary>
        /// 获取单元上一次的审批人
        /// </summary>
        public GetRreTaskUserMethod GetPrevTaskUser { get; set; }

        /// <summary>
        /// 获取系统管理员
        /// </summary>
        public GetSystemUserListMethod GetSystemUserList { get; set; }
        /// <summary>
        /// 判断会签是否通过
        /// </summary>
        public IsCountersignAgreeMethod IsCountersignAgree { get; set; }
        /// <summary>
        /// 获取上一个流入节点（不是驳回流入的）
        /// </summary>
        public GetPrevUnitIdMethod GetPrevUnitId { get; set; }

        /// <summary>
        /// 获取流程表单信息
        /// </summary>
        public string GetFormTableInfo { get; set; }
    }
}
