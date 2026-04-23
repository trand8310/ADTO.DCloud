using System;
using System.ComponentModel;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.Customers.Dto
{
    /// <summary>
    /// 客户信息
    /// </summary>
    [AutoMap(typeof(Customer))]
    public class CustomerDto:EntityDto<Guid>
    {
        /// <summary>
        /// 客户编码
        /// </summary>
        [Description("客户编码")]
        public string Code { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        [Description("客户名称")]
        public string Name { get; set; }

        /// <summary>
        /// 公司规模(字典value)
        /// </summary>
        [Description("公司规模")]
        public string ScaleType { get; set; }

        /// <summary>
        /// 公司邮箱
        /// </summary>
        [Description("公司邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 客户电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 客户来源
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 公司类型 字典
        /// </summary>
        public string CustomerType { get; set; }

        /// <summary>
        /// 客户状态 字典/-
        /// </summary>
        public string CustomerState { get; set; }

        /// <summary>
        /// 客户状态 字典
        /// </summary>
        public string CustomerStateName { get; set; }

        /// <summary>
        /// 成立时间
        /// </summary>
        //[DisableDateTimeNormalization]
        //[JsonConverter(typeof(OnlyDateConverter))]
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// 主营行业 字典
        /// </summary>
        public string IndustryType { get; set; }

        /// <summary>
        /// 客户评级 字典
        /// </summary>
        public string CustomerLevel { get; set; }

        /// <summary>
        /// 客户评级 字典
        /// </summary>
        public string CustomerLevelName { get; set; }

        /// <summary>
        /// 年营业额
        /// </summary>
        public decimal AnnualAales { get; set; }

        /// <summary>
        /// 公司网址
        /// </summary>
        public string WebSite { get; set; }

        /// <summary>
        /// Whatapp
        /// </summary>
        public string Whatapp { get; set; }

        /// <summary>
        /// 所在地区
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 所属国家
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// 所属省份
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// 所属市州
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 所属区县
        /// </summary>
        public int CountyId { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 地区国家省市区冗余字段
        /// </summary>
        public string LocationStr { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public bool AuditStatus { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditTime { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public Guid AuditUserId { get; set; }

        /// <summary>
        /// 所属人
        /// </summary>
        public Guid ResponsibilityUserId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreatorUserId { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string CreatorUserName { get; set; }

        /// <summary>
        /// 最后跟进时间
        /// </summary>
        public DateTime? LastFollowTime { get; set; }

        /// <summary>
        /// 联系人电话（第一条）
        /// </summary>
        public string FirstContactPhone { get; set; }

        /// <summary>
        /// 附件夹Id
        /// </summary>
        public Guid? FolderId { get; set; }
    }
}
