using ADTO.DCloud.Project;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 项目日志
    /// </summary>
    [AutoMap(typeof(ProjectLog))]
    public class ProjectLogDto 
    {
        /// <summary>
        /// 所属项目
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 操作类型 字典（新增、修改、分享、审核、跟进 等所有操作）
        /// </summary>
        public string OperateType { get; set; }

        /// <summary>
        /// 数据详情
        /// </summary>
        public string DataDetail { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid? CreatorUserId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorUserName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
