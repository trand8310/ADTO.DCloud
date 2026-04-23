using ADTO.DCloud.WorkFlow.Processs.Config;
using ADTO.DCloud.WorkFlow.Processs.Dto;
using ADTO.DCloud.WorkFlow.Tasks;
using ADTOSharp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs
{
    /// <summary>
    /// 工作流引擎
    /// </summary>
    public interface IWorkFlowEngineAppService : IApplicationService
    {
        /// <summary>
        /// 开始节点
        /// </summary>
        WorkFlowUnit StartNode { get; }

        /// <summary>
        /// 结束节点
        /// </summary>
        WorkFlowUnit EndNode { get; }

        /// <summary>
        /// 流程运行参数
        /// </summary>
        WFEngineConfig Config { get; }


        /// <summary>
        /// 流程发起用户
        /// </summary>
        WFUserInfo CreateUser { get; }


        /// <summary>
        /// 流程配置信息
        /// </summary>
        WFScheme WFScheme { get; }

        /// <summary>
        /// 获取流程单元信息
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>流程单元信息</returns>
        WorkFlowUnit GetNode(Guid id);

        /// <summary>
        /// 获取下一节点集合
        /// </summary>
        /// <param name="startId">开始节点</param>
        /// <param name="code">执行动作编码</param>
        /// <returns></returns>
        List<WorkFlowUnit> GetNextUnits(Guid startId, string code = "");

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="startId">开始节点</param>
        /// <param name="isRejectBackOld">是否原路返回 1 是 其它否</param>
        /// <param name="code">执行动作编码</param>
        /// <param name="toUnitId">下一个指定节点</param>
        /// <param name="rejectNode">驳回节点</param>
        /// <param name="fromId">上一个节点</param>
        /// <param name="IsGetNextAuditors">用来获取下一审批人</param>
        /// <returns></returns>
        Task<List<WFTask>> GetTask(Guid startId, int? isRejectBackOld, string code="", Guid? toUnitId = null, Guid? rejectNode=null, Guid? fromId = null , bool IsGetNextAuditors = false);

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="nextUnits">下一个节点集合</param>
        /// <param name="isReject">是否驳回</param>
        /// <param name="startId">开始节点</param>
        /// <param name="isRejectBackOld">是否原路返回 1 是 其它否</param>
        /// <param name="IsGetNextAuditors">用来获取下一审批人</param>
        /// <returns></returns>
        public Task<List<WFTask>> GetTask1(List<WorkFlowUnit> nextUnits, bool isReject, Guid startId, int? isRejectBackOld, bool IsGetNextAuditors);

        /// <summary>
        /// 初始化工作流引擎（代替原来的构造函数逻辑）
        /// </summary>
        void Initialize(WFEngineConfig config);
    }
}
