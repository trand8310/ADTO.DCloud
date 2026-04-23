using ADTO.DCloud.WorkFlow.Processs.Dto;
using ADTO.DCloud.WorkFlow.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Config
{
    /// <summary>
    /// 获取等待任务
    /// </summary>
    /// <param name="processId">流程进程ID</param>
    /// <param name="unitId">流程单元ID</param>
    /// <returns></returns>
    public delegate Task<List<WFTask>> GetAwaitTaskListMethod(Guid processId, Guid unitId);
    /// <summary>
    /// 获取设置人员
    /// </summary>
    /// <param name="auditorList">人员配置信息</param>
    /// <param name="createUser">流程发起人</param>
    /// <param name="processId">流程进程实例ID</param>
    /// <param name="myNode">当前节点</param>
    /// <param name="preNode">上一个节点</param>
    /// <param name="startNode">开始节点</param>
    /// <returns></returns>
    public delegate Task<List<WFUserInfo>> GetUserListMethod(List<WorkFlowAuditor> auditorList, WFUserInfo createUser, Guid processId, WorkFlowUnit myNode, WorkFlowUnit preNode, WorkFlowUnit startNode);
    /// <summary>
    /// 获取系统管理员
    /// </summary>
    /// <returns></returns>
    public delegate Task<List<WFUserInfo>> GetSystemUserListMethod();

    /// <summary>
    /// 获取单元上一次的审批人
    /// </summary>
    /// <param name="processId">流程进程ID</param>
    /// <param name="unitId">流程单元ID</param>
    /// <returns></returns>
    public delegate Task<List<WFUserInfo>> GetRreTaskUserListMethod(Guid processId, Guid unitId);

    /// <summary>
    /// 获取单元上一次的审批人
    /// </summary>
    /// <param name="processId">流程进程ID</param>
    /// <param name="unitId">流程单元ID</param>
    /// <returns></returns>
    public delegate Task<WFUserInfo> GetRreTaskUserMethod(Guid processId, Guid unitId);

    /// <summary>
    /// 判断会签是否通过
    /// </summary>
    /// <param name="processId">流程进程ID</param>
    /// <param name="unitId">流程单元ID</param>
    /// <returns></returns>
    public delegate Task<bool> IsCountersignAgreeMethod(Guid processId, Guid unitId);

    /// <summary>
    /// 获取上一个流入节点（不是驳回流入的）
    /// </summary>
    /// <param name="processId">流程进程ID</param>
    /// <param name="unitId">流程单元ID</param>
    /// <param name="fromId">上一个节点ID</param>
    /// <returns></returns>
    public delegate Task<Guid> GetPrevUnitIdMethod(Guid processId, Guid unitId, Guid fromId);
}
