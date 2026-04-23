using System;
using ADTOSharp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.DataItem.Dto
{
    /// <summary>
    /// 添加字典
    /// </summary>
    [AutoMapTo(typeof(DataItem))]
    public class CreateDataItemInputDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 父级主键
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [Required(ErrorMessage = "分类名称不能为空")]
        public string ItemName { get; set; }

        /// <summary>
        /// 分类编码
        /// </summary>
        [Required(ErrorMessage = "分类编码不能为空")]
        public string ItemCode { get; set; }
       
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 租户
        /// </summary>
        public Guid? TenantId { get; set; }
    }
}
