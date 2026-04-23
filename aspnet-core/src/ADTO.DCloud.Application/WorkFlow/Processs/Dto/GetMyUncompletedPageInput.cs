using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 获取我的代办任务
    /// </summary>
    public class GetMyUncompletedPageInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束流程
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 是否允许批量审批
        /// </summary>
        public bool IsBatchAudit { get; set; } = false;
        /// <summary>
        /// 流程模板编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 流程模板名称
        /// </summary>
        public string SchemaName { get; set; }
        /// <summary>
        /// 流程Id
        /// </summary>
        public string ProcessId { get; set; }
        /// <summary>
        /// 类型 1运行2草稿3作废4结束
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 表单关键字
        /// </summary>
        public string FormKeyword { get; set; }
        /// <summary>
        /// 查询条件
        /// </summary>
        public string Where { get; set; }

        //扩展查询字段【流程】
        /// <summary>
        /// 发起人Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 流程模板Id
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 单位Id
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 部门及下属部门
        /// </summary>
        public List<string> CompanyList { get; set; }
        //扩展查询字段【任务】----ADD BY SSY 20230425
        /// <summary> 
        /// 1.审批任务
        /// 2.传阅任务
        /// 3.子流程
        /// 4.重新创建
        /// 5.会签任务
        /// 6.加签审批
        /// 7.沟通审批 
        /// 10.脚本任务
        /// 21.等待任务（系统自己完成）
        /// 22.取消等待任务
        /// 23.找不到审批人直接跳过
        /// 24.自动审批规则跳过
        /// 25.会签任务记录
        /// </summary> 
        public int? TaskType { get; set; }
        /// <summary>
        /// 任务执行人id
        /// </summary>
        public string TaskUserId { get; set; }
        /// <summary>
        /// 任务状态 1.激活 2.待激活 3.完成 4.关闭5.加签状态
        /// </summary>
        public int? TaskState { get; set; }
        /// <summary>
        /// 流程任务节点
        /// </summary>
        public string UnitId { get; set; }

        public string Filter { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime";
            }
            Filter = Filter?.Trim();
        }

    }
}
