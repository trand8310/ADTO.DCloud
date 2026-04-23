using ADTOSharp.Application.Services.Dto;
using System;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTO.DCloud.FormScheme.Dto
{
    /// <summary>
    /// 表单模板表
    /// 每次更新新增一条数据，一对多
    /// </summary>
    [AutoMap(typeof(FormScheme))]
    public class FormSchemeDto : EntityDto<Guid>, IHasCreationTime
    {
        /// <summary>
        /// 模板信息主键
        /// </summary>
        public Guid SchemeInfoId { get; set; }

        /// <summary>
        /// 1.正式2.草稿
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorUserName { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
