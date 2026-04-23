using ADTO.DCloud.WorkFlow.Processs.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Config
{
    /// <summary>
    /// 工作流模板模型
    /// </summary>
    public class WFScheme
    {
        /// <summary>
        /// 流程图数据
        /// </summary>
        public List<WorkFlowUnit> WfData { get; set; }
        /// <summary>
        /// 撤销执行 动作类型 1执行SQL 2.NET方法 3第三方接口
        /// </summary>
        public string UndoType { get; set; }
        /// <summary>
        /// 撤销执行 数据库编码
        /// </summary>
        public string UndoDbCode { get; set; }
        /// <summary>
        /// 撤销执行 SQL语句
        /// </summary>
        public string UndoDbSQL { get; set; }
        /// <summary>
        /// 撤销执行 .NET注入类名
        /// </summary>
        public string UndoIOCName { get; set; }
        /// <summary>
        /// 撤销执行 接口地址
        /// </summary>
        public string UndoUrl { get; set; }


        /// <summary>
        /// 作废操作 动作类型 1执行SQL 2.NET方法 3第三方接口
        /// </summary>
        public string DeleteType { get; set; }
        /// <summary>
        /// 作废操作 数据库编码
        /// </summary>
        public string DeleteDbCode { get; set; }
        /// <summary>
        /// 作废操作 SQL语句
        /// </summary>
        public string DeleteDbSQL { get; set; }
        /// <summary>
        /// 作废操作 .NET注入类名
        /// </summary>
        public string DeleteIOCName { get; set; }
        /// <summary>
        /// 作废操作 接口地址
        /// </summary>
        public string DeleteUrl { get; set; }

        /// <summary>
        /// 删除草稿 动作类型 1执行SQL 2.NET方法 3第三方接口
        /// </summary>
        public string DeleteDraftType { get; set; }
        /// <summary>
        /// 删除草稿 数据库编码
        /// </summary>
        public string DeleteDraftDbCode { get; set; }
        /// <summary>
        /// 删除草稿 SQL语句
        /// </summary>
        public string DeleteDraftDbSQL { get; set; }
        /// <summary>
        /// 删除草稿 .NET注入类名
        /// </summary>
        public string DeleteDraftIOCName { get; set; }
        /// <summary>
        /// 删除草稿 接口地址
        /// </summary>
        public string DeleteDraftUrl { get; set; }

        /// <summary>
        /// 数据库（保存关键字）
        /// </summary>
        public string KeywordDb { get; set; }
        /// <summary>
        ///  数据表（保存关键字）
        /// </summary>
        public string KeywordTable { get; set; }
        /// <summary>
        ///  数据关联（保存关键字）
        /// </summary>
        public string KeywordRField { get; set; }
        /// <summary>
        ///  数据保存字段（保存关键字）
        /// </summary>
        public string KeywordSField { get; set; }


    }
}
