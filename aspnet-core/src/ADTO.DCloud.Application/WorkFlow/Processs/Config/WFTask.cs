using ADTO.DCloud.WorkFlow.Processs.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Config
{
    /// <summary>
    /// 流程任务
    /// </summary>
    public class WFTask
    {
        /// <summary>
        /// 流程单元ID
        /// </summary>
        public Guid UnitId { get;set;}
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 任务令牌
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 任务类型
        /// 1.审批任务
        /// 2传阅任务
        /// 3.子流程
        /// 4.重新创建
        /// 5.会签任务
        /// 10.脚本任务
        /// 21.等待任务（系统自己完成）
        /// 22.取消等待任务
        /// 23.找不到审批人直接跳过
        /// 24.自动审批规则跳过
        /// 25.会签任务记录
        /// 26 更新任务状态
        /// 27 找不审批人转到发起人
        /// 99 当前节点已经在审批无需处理
        /// 100 结束任务 101 结束节点消息任务
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 上一个流程单元ID
        /// </summary>
        public Guid PrevUnitId { get; set; }
        /// <summary>
        /// 上一个流程单元名称
        /// </summary>
        public string PrevUnitName { get; set; }
        /// <summary>
        /// 是否是驳回任务
        /// </summary>
        public bool IsReject { get; set; }
        /// <summary>
        /// 是否允许批量审批
        /// </summary>
        public bool IsBatchAudit { get; set; }

        /// <summary>
        /// 通知策略 1.短信 2.邮箱 3.微信 4.IM
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// 是否超时通知
        /// </summary>
        public bool IsOvertimeMessage { get; set; }
        /// <summary>
        /// 第一次通知 单位（时）
        /// </summary>
        public int OvertimeMessageStart { get; set; }
        /// <summary>
        /// 间隔通知 单位（时）
        /// </summary>
        public int OvertimeMessageInterval { get; set; }
        
        /// <summary>
        /// 超时流转
        /// </summary>
        public bool IsOvertimeGo{ get; set; }
        
        /// <summary>
        /// 超时流转时间 单位（时）
        /// </summary>
        public int OvertimeGo { get; set; }
        /// <summary>
        /// 超时通知策略 1.短信 2.邮箱 3.微信 4.IM
        /// </summary>
        public string OvertimeMessageType { get; set; }

        /// <summary>
        /// 脚本执行类型 1SQL 2接口 3IOC
        /// </summary>
        public string ExecuteType { get; set; }
        /// <summary>
        /// 执行SQL数据库编码
        /// </summary>
        public string SqlDb { get; set; }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        public string SqlStr { get; set; }
        /// <summary>
        /// 撤回的时候执行SQL语句
        /// </summary>
        public string SqlStrRevoke { get; set; }
        /// <summary>
        /// 调用接口
        /// </summary>
        public string ApiUrl { get; set; }
        /// <summary>
        /// 撤回的时候调用接口
        /// </summary>
        public string ApiUrlRevoke { get; set; }
        /// <summary>
        /// 注入类名
        /// </summary>
        public string Ioc { get; set; }
        /// <summary>
        /// 撤回的时候注入类名
        /// </summary>
        public string IocRevoke { get; set; }

        /// <summary>
        /// 处理人信息
        /// </summary>
        public WFUserInfo User { get; set; }
        

        /// <summary>
        /// 是否自动创建子流程
        /// </summary>
        public bool IsAuto { get; set; }
        /// <summary>
        /// 流程模板ID
        /// </summary>
        public Guid WfschemeId { get; set; }
        /// <summary>
        /// 流程模板版本
        /// </summary>
        public Guid WfVersionId { get; set; }
        /// <summary>
        /// 子流程id
        /// </summary>
        public Guid ChildProcessId { get; set; }

        /// <summary>
        /// 审批步骤
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// 审批时长
        /// </summary>
        public int AuditTime { get; set; }
        
        /// <summary>
        /// 节点编码
        /// </summary>
        public string NodeCode  { get; set; }
    }
}
