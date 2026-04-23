using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 审批人员设置
    /// </summary>
    public class WorkFlowAuditor
    {
        /// <summary>
        /// 设置Id 1.上一级 2.上二级 3.上三级 4.上四级 5.上五级 6.下一级 7.下二级 8.下三级 9.下四级 10.下五级
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 审批人员类型 1.岗位 2.角色 3.用户 4.上下级 5.节点执行人 6.数据库表字段 7.发起人部门 8.上一审批人部门 9.特定部门 11 上一审批人公司 12 特定公司 13 sql语句
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 条件 1.同一个部门 2.同一个公司 3.发起人上级 4.发起人下级 
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public Guid Company { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public Guid Department { get; set; }

        /// <summary>
        /// 数据库编码
        /// </summary>
        public string DbCode { get; set; }
        /// <summary>
        /// 对应表
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// 关联字段
        /// </summary>
        public string Rfield { get; set; }
        /// <summary>
        /// 审批人字段
        /// </summary>
        public string AuditorField { get; set; }
        /// <summary>
        /// 上下级
        /// </summary>
        public string Level { get; set; }
        /// <summary>
        /// 节点id
        /// </summary>
        public Guid Enforcer { get; set; }
        /// <summary>
        /// sql
        /// </summary>
        public string StrSql { get; set; }


    }
}
