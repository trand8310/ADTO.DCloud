using System;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTO.DCloud.FormScheme.Dto
{
    /// <summary>
    /// 动态表单主表记录
    /// </summary>
    [AutoMap(typeof(FormSchemeInfo))]
    public class FormSchemeInfoDto : EntityDto<Guid?>, IHasCreationTime
    {
        /// <summary>
        /// 表单名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 表单分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 当前模板主键
        /// </summary>
        public Guid SchemeId { get; set; }

        /// <summary>
        /// 表单类型 1 视图表单 0其他数据库表单 2自动建表
        /// </summary>
        public int? FormType { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 自定义服务方法-通过对应的服务方法来保存表单信息
        /// </summary>
        public string MethodName { get; set; }
        public DateTime CreationTime { get; set; }

    }
}
