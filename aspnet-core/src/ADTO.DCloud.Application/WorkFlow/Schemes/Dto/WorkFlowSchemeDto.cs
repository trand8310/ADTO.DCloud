using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapFrom(typeof(WorkFlowScheme))]
    public class WorkFlowSchemeDto : Entity<Guid>, ICreationAudited
    {
        #region 实体成员 
        /// <summary> 
        /// 流程模板信息主键 
        /// </summary> 
        public Guid SchemeInfoId { get; set; }
        /// <summary> 
        /// 1.正式2.草稿 
        /// </summary> 
        public int? Type { get; set; }
        /// <summary> 
        /// 流程模板内容 
        /// </summary> 
        [StringLength(int.MaxValue)]
        public string Content { get; set; }
        #endregion
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorUserName { get; set; }
        #region 
        public Guid? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        #endregion
    }
}
