using Castle.Components.DictionaryAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 流程单元
    /// </summary>
    public class WorkFlowUnit
    {
        #region 节点原始属性
        /// <summary>
        /// 流程单元类型 
        /// 线条 myline 
        /// 开始节点 startEvent 
        /// 结束节点 endEvent 
        /// 并行网关 gatewayAnd 并行网关会等待所有分支汇入才往下执行，所有出口分支都会被执行
        /// 排他网关 gatewayXor 排他网关不会等待所有分支汇入才往下执行，只要有分支汇入就会往下执行，出口分支只会执行一条（条件为true，如果多条出口分支条件为true也执行一条）
        /// 包容网关 gatewayInclusive  包容网关会等待所有分支汇入才往下执行，出口分支能执行多条（条件为true）
        /// 审批节点 userTask 
        /// 脚本节点 scriptTask 
        /// 子流程节点 subprocess
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 流程单元id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 流出节点ID
        /// </summary>
        public Guid From { get; set; }
        /// <summary>
        /// 流入节点ID
        /// </summary>
        public Guid To { get; set; }
        #endregion
        /// <summary>
        /// 通知策略 1.短信 2.邮箱 3.微信 4.IM
        /// </summary>
        public string MessageType { get; set; }
        /// <summary>
        /// 条件节点执行条件
        /// </summary>
        public List<WorkFlowCondition> Conditions { get; set; }
        /// <summary>
        /// 是否允许加签
        /// </summary>
        public bool IsAddSign { get; set; }
        /// <summary>
        /// 是否允许批量审批
        /// </summary>
        public bool IsBatchAudit { get; set; }
        /// <summary>
        /// 会签节点驳回通过后返回我
        /// </summary>
        public bool IsRejectBackOld { get; set; }

        /// <summary>
        /// 自动同意规则 1.处理人就是提交人 2.处理人和上一步的处理人相同 3.处理人审批过
        /// </summary>
        public string AutoAgree { get; set; }
        /// <summary>
        /// 无对应处理人 1.超级管理员处理 2.跳过此步骤 3.不能提交
        /// </summary>
        public string NoAuditor { get; set; }
        /// <summary>
        /// 驳回策略 1.驳回节点固定 2.能驳回到任何执行过节点
        /// </summary>
        public string RejectType { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public List<WorkFlowAuditor> AuditUsers { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public List<WorkFlowAuditor> AuditorUsersCard { get; set; }
        /// <summary>
        /// 传阅人
        /// </summary>
        public List<WorkFlowAuditor> LookUsers { get; set; }
        /// <summary>
        /// 传阅人
        /// </summary>
        public List<WorkFlowAuditor> LookUsersCard { get; set; }
        /// <summary>
        /// 是否会签
        /// </summary>
        public bool IsCountersign { get; set; }
        /// <summary>
        /// 是否等待所有人审批完
        /// </summary>
        public bool IsCountersignAll { get; set; }
        /// <summary>
        /// 通过百分比
        /// </summary>
        public int CountersignAllType { get; set; }
        /// <summary>
        /// 审核方式（1只要其中一人审核2都需要审核）---如果需要2人审批，可以把节点设置成会签
        /// </summary>
        //public string isAllAuditor { get; set; }
        /// <summary>
        /// 审批方式 1.并行 2.串行
        /// </summary>
        public string CountersignType { get; set; }
        /// <summary>
        /// 再次审批 1.已同意不需要审批 2.已同意需要审批
        /// </summary>
        public string CountersignAgian { get; set; }

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
        /// 是否超时流转
        /// </summary>
        public bool IsOvertimeGo { get; set; }
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
        /// 线条流转条件
        /// </summary>
        public string LineConditions { get; set; }

        /// <summary>
        /// 是否每次审批，审批人重新获取
        /// </summary>
        public bool IsUpdateAuditor { get; set; }

        /// <summary>
        /// 表单类型 1 自定义表单，其它系统表单
        /// </summary>
        public string FormType { get; set; }
        /// <summary>
        /// 表单编码
        /// </summary>
        public string FormCode { get; set; }
        /// <summary>
        /// 表单版本
        /// </summary>
        public string FormVerison { get; set; }
        /// <summary>
        /// 表单关联字段
        /// </summary>
        public string FormRelationId { get; set; }


        /// <summary>
        /// 是否开启自定义标题
        /// </summary>
        public bool IsCustmerTitle { get; set; }
        /// <summary>
        /// 自定义标题配置信息
        /// </summary>
        public List<WorkFlowCustmerTitleRule> TitleRule { get; set; }

        /// <summary>
        /// 审批步骤
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// 审批时长
        /// </summary>
        public int AuditTime { get; set; }

        /// <summary>
        /// 总的审批时长
        /// </summary>
        public int AllAuditTime { get; set; }

        /// <summary>
        /// 节点编码
        /// </summary>
        public string NodeCode { get; set; }

        /// <summary>
        /// 节点-执行服务方法
        /// </summary>
        public string ServiceInvokeMethod { get; set; }
        /// <summary>
        /// 节点-执行回调方法
        /// </summary>
        public string ServerCallbackFun { get; set; }
        /// <summary>
        /// 节点-执行服务方法参数
        /// </summary>
        public List<WorkFlowCustmerTitleRule> ServerCallbackFunParameters { get; set; }
    }
}
