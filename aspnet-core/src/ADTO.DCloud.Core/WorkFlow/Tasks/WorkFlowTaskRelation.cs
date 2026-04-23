using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Tasks
{
    /// <summary> 
    ///  日 期：2024-12-27
    /// 描 述：工作流任务执行人对应关系表 (数据库没有创建)
    /// </summary>
    [Table("WorkFlowTaskRelations")]
    public class WorkFlowTaskRelation:Entity<Guid>, ICreationAudited, IMayHaveTenant
    {
        #region 实体成员 
        /// <summary>  
        /// 任务主键 
        /// </summary> 
        public Guid TaskId { get; set; }
        /// <summary> 
        /// 任务执行人员主键 
        /// </summary> 
        public Guid? UserId { get; set; }
        /// <summary>
        /// 标记0需要处理1暂时不需要处理
        /// </summary>
        public int? Mark { get; set; }
        /// <summary>
        /// 处理结果0.未处理1.同意2.不同意3.超时4.其他
        /// </summary>
        public int? Result { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 任务执行时间
        /// </summary>
        public DateTime? Time { get; set; }
        public Guid? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }

        #endregion
        #region 多租户

        /// <summary>
        /// 多租户
        /// </summary>
        public Guid? TenantId { get; set; }
        #endregion
    }
}
