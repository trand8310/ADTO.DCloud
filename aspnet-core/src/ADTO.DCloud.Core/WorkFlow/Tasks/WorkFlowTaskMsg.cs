using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Tasks
{
    /// <summary>
    /// 2024-12-27
    ///  描 述：工作流任务消息
    /// </summary>
    [Table("WorkFlowTaskMsg")]
    public class WorkFlowTaskMsg : EntityDto<Guid>, ICreationAudited, IRemark, IMayHaveTenant
    {
        #region 实体成员 
        /// <summary> 
        /// 流程进程主键 
        /// </summary> 
        public Guid ProcessId { get; set; }
        /// <summary> 
        /// 流程任务主键 
        /// </summary> 
        public Guid TaskId { get; set; }
        /// <summary> 
        /// 任务发送人主键 
        /// </summary> 
        public Guid FromUserId { get; set; }
        /// <summary> 
        /// 任务发送人账号 
        /// </summary> 
        [StringLength(50)]
        public string FromUserAccount { get; set; }
        /// <summary> 
        /// 任务发送人名称 
        /// </summary> 
        [StringLength(50)]
        public string FromUserName { get; set; }
        /// <summary> 
        /// 任务接收人主键 
        /// </summary> 
        public Guid ToUserId { get; set; }
        /// <summary> 
        /// 任务接收人账号 
        /// </summary> 
        [StringLength(50)]
        public string ToAccount { get; set; }
        /// <summary> 
        /// 任务接收人名称 
        /// </summary> 
        [StringLength(50)]
        public string ToName { get; set; }
        /// <summary> 
        /// 任务标题 
        /// </summary> 
        [StringLength(125)]
        public string Title { get; set; }
        /// <summary> 
        /// 任务内容 
        /// </summary> 
        [StringLength(500)]
        public string Remark {  get; set; }
        #endregion

        #region 多租户

        /// <summary>
        /// 多租户
        /// </summary>
        public Guid? TenantId { get; set; }
        public Guid? CreatorUserId {  get; set; }
        public DateTime CreationTime {  get; set; }
        #endregion
    }
}
