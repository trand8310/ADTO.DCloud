using ADTO.DCloud.DatabaseManager;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.CodeRule.Dto
{
    /// <summary>
    /// 添加编码配置信息
    /// </summary>
    [AutoMapTo(typeof(CodeRule))]
    public class CreateCodeRuleDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Required(ErrorMessage ="编号不能为空")]
        public string RuleCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        public string RuleName { get; set; }

        /// <summary>
        /// 日期类型
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// 编码前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 编码后缀
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// 流水号长度   
        /// </summary>
        public int SeqLength { get; set; }

        /// <summary>
        /// 初始值
        /// </summary>
        public int? InitSeq { get; set; } = 1;

        /// <summary>
        /// 当前序号（用于自增）
        /// </summary>
        public int CurrentSeq { get; set; }

        /// <summary>
        ///是否按日期重置流水号（开关）
        /// </summary>
        public bool IsResetByDate { get; set; } = false; // 默认不重置

        /// <summary>
        /// 最后一次重置流水号的日期标识（如 20260318/202603，和DateFormat格式一致）
        /// </summary>
        public string LastResetDate { get; set; }

        /// <summary>
        /// 连接符
        /// </summary>
        public string SegmentSeparator { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 是否有效、启用
        /// </summary>
        [Description("是否有效、启用")]
        public bool IsActive { get; set; }

    }
}
