using ADTO.DCloud.Project;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
 
namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 新增项目日志
    /// </summary>
    [AutoMapTo(typeof(ProjectLog))]
    public class CreateProjectLogDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 所属项目
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 操作类型 字典（新增、修改、审核、跟进 等所有操作）
        /// </summary>
        public string OperateType { get; set; }

        /// <summary>
        /// 数据详情
        /// </summary>
        public string DataDetail { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid? CreatorUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
