using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.ExcelManager.Import.Dto
{
    /// <summary>
    /// 添加Excel 导入字段信息
    /// </summary>
    [AutoMapTo(typeof(ExcelImportField))]
    public class CreateExcelImportFieldDto : EntityDto<Guid?>
    {
        /// <summary>
        ///  配置主表Id
        /// </summary>
        [Description("配置主表Id")]
        public Guid ImportId { get; set; }

        /// <summary>
        /// 表字段名
        /// </summary>
        [Required(ErrorMessage = "表字段名不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// Excel列名
        /// </summary>
        [Required(ErrorMessage = "Excel列名不能为空")]
        public string ColName { get; set; }

        /// <summary>
        /// 是否唯一性验证:false不要要,true需要  默认不需要
        /// </summary>
        public bool? IsOnlyOne { get; set; }

        /// <summary>
        /// 转换类型（0=普通；1=字典；2=表外键）
        /// </summary>
        public int ConvertType { get; set; }

        /// <summary>
        /// 字典类型编码
        /// </summary>
        public string DictTypeCode { get; set; }

        /// <summary>
        /// 外键关联实体表名
        /// </summary>
        public string RelationEntity { get; set; }

        /// <summary>
        /// 外键匹配字段
        /// </summary>
        public string MatchField { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Description("创建人")]
        public Guid? CreatorUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreationTime { get; set; }

        
    }
}
